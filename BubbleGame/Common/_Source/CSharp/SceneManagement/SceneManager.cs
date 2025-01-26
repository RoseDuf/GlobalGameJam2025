using Godot;

using System.IO;
using System.Collections.Generic;
using System;
using Godot.Collections;

namespace BubbleGame.Common.SceneManagement
{
	public partial class SceneManager : SingletonNode<SceneManager>
	{
		[Export]
		public SceneCollection SceneCollection { get; private set; }

		public Node PersistentRoot { get; private set; }

		private List<Node> _additiveSceneNodes = new List<Node>();

		protected override void Initialize()
		{
			base.Initialize();
			
			PersistentRoot = new Node();
			GetTree().Root.CallDeferred(Node.MethodName.AddChild, PersistentRoot);

			SceneCollection.Initialize();
		}

		public void PersistNode(Node node)
		{
			if (!IsPersistent(node))
				PersistentRoot.CallDeferred(Node.MethodName.AddChild, node);
		}

		public void RemoveNode(Node node)
		{
			if (IsPersistent(node))
			{
				PersistentRoot.CallDeferred(Node.MethodName.RemoveChild, node);
				node.QueueFree();
			}
		}

		public bool IsPersistent(Node node)
		{
			return node.GetParent() == PersistentRoot;
		}

		/// <summary>
		/// Attempts to load a referenced scene from the scene collection by index.
		/// </summary>
		public void LoadScene(int index, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
		{
			if (SceneCollection.TryGetPackedSceneByIndex(index, out PackedScene packedScene))
				LoadScene(packedScene, loadSceneMode);
		}

		/// <summary>
		/// Attempts to load a referenced scene from the scene collection by name or path.
		/// Note: It is <b><u>strongly advised</u></b> to NOT have multiple scenes with the same name in the scene collection.
		/// </summary>
		public void LoadScene(string nameOrPath, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
		{
			if (nameOrPath.EndsWith(".tscn"))
			{
				if (SceneCollection.TryGetPackedSceneByPath(nameOrPath, out PackedScene packedScene))
					LoadScene(packedScene, loadSceneMode);
			}
			else if (SceneCollection.TryGetPackedSceneByName(nameOrPath, out PackedScene packedScene))
			{
				LoadScene(packedScene, loadSceneMode);
			}
		}

		/// <summary>
		/// Attempts to load a referenced scene from the scene collection by index.
		/// </summary>
		public void LoadScene(PackedScene packedScene, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
		{
			if (loadSceneMode == LoadSceneMode.Single)
			{
				CallDeferred(nameof(DeferredChangeCurrentSceneToPacked), packedScene);
			}
			else if (loadSceneMode == LoadSceneMode.Additive)
			{
				CallDeferred(nameof(DeferredInstantiatePackedScene), packedScene);
			}
		}

		private void DeferredChangeCurrentSceneToPacked(PackedScene packedScene)
		{
			SceneTree sceneTree = GetTree();
			foreach (Node additiveSceneNode in _additiveSceneNodes)
			{
				string scenePathOrName = string.IsNullOrWhiteSpace(additiveSceneNode.SceneFilePath) ? additiveSceneNode.Name : additiveSceneNode.SceneFilePath;
				GD.Print($"[{nameof(SceneManager)}] Unloading Scene: {scenePathOrName}");

				sceneTree.Root.RemoveChild(additiveSceneNode);
				additiveSceneNode.Free();
			}

			if (sceneTree.CurrentScene != null)
			{
				GD.Print($"[{nameof(SceneManager)}] Loading Scene: {packedScene.ResourcePath}");

				string error = "Failed to change CurrentScene.";
				Error errorCode = Error.Ok;

				if (sceneTree.CurrentScene.SceneFilePath != null && sceneTree.CurrentScene.SceneFilePath == packedScene.ResourcePath)
					errorCode = sceneTree.ReloadCurrentScene();
				else
					errorCode = sceneTree.ChangeSceneToPacked(packedScene);

				if (errorCode != Error.Ok)
					GD.PushError($"{error} ErrorCode: {errorCode}");
			}
			else
			{
				Node sceneNode = packedScene.Instantiate();
				sceneTree.Root.AddChild(sceneNode);
				sceneTree.CurrentScene = sceneNode;
			}
		}

		private void DeferredInstantiatePackedScene(PackedScene packedScene)
		{
			Node sceneNode = packedScene.Instantiate();
			GetTree().Root.AddChild(sceneNode);
			_additiveSceneNodes.Add(sceneNode);
		}

		public void UnloadScene(int index)
		{
			if (SceneCollection.TryGetPackedSceneByIndex(index, out PackedScene packedScene))
			{
				Node root = GetTree().Root;
				if (packedScene.ResourcePath != null)
				{
					string sceneName = Path.GetFileNameWithoutExtension(packedScene.ResourcePath);
					Node sceneNode = root.GetNode(sceneName);
					UnloadScene(sceneNode);
				}
			}
		}

		public void UnloadScene(string nameOrPath)
		{
			Node root = GetTree().Root;
			if (nameOrPath.EndsWith(".tscn"))
			{
				if (SceneCollection.TryGetPackedSceneByPath(nameOrPath, out PackedScene packedScene) && packedScene.ResourcePath != null)
				{
					string sceneName = Path.GetFileNameWithoutExtension(packedScene.ResourcePath);
					Node sceneNode = root.GetNode(sceneName);
					UnloadScene(sceneNode);
				}
			}
			else if (SceneCollection.TryGetPackedSceneByName(nameOrPath, out PackedScene packedScene))
			{
				Node sceneNode = root.GetNode(nameOrPath);
				UnloadScene(sceneNode);
			}
		}

		public void UnloadScene(Node sceneNode)
		{
			CallDeferred(nameof(DeferredRemoveSceneNode), sceneNode);
		}

		private void DeferredRemoveSceneNode(Node sceneNode)
		{
			SceneTree sceneTree = GetTree();

			sceneTree.Root.RemoveChild(sceneNode);
			sceneNode.Free();

			if (sceneNode == sceneTree.CurrentScene)
				sceneTree.CurrentScene = null;
		}

		public Godot.Collections.Array GetTimeStampsCachedData()
		{
            var timeStamps = new Godot.Collections.Array();
            var config = new ConfigFile();

            // Load data from a file.
            Error err = config.Load("user://levelcache.cfg");

            // If the file didn't load, ignore it.
            if (err != Error.Ok)
            {
                return null;
            }

            timeStamps = (Godot.Collections.Array)config.GetValue("CachedData", "TimeStamps");

			return timeStamps;
        }

        public float GetTimeElapsedCachedData()
        {
			float timeElapsed = 0;
            var config = new ConfigFile();

            // Load data from a file.
            Error err = config.Load("user://levelcache.cfg");

            // If the file didn't load, ignore it.
            if (err != Error.Ok)
            {
                return 0;
            }

            timeElapsed = (float)config.GetValue("CachedData", "TimeElapsed");

			return timeElapsed;	
        }
    }
}
