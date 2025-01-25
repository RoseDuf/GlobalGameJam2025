using Godot;

using System.IO;
using System.Collections.Generic;

namespace BubbleGame.Common.SceneManagement
{
	public partial class SceneCollection : Resource
	{
		[Export]
		private SceneInfo[] _scenes;

		private Dictionary<string, SceneInfo> _pathToSceneInfoTable = new Dictionary<string, SceneInfo>();
		private Dictionary<string, SceneInfo> _nameToSceneInfoTable = new Dictionary<string, SceneInfo>();

		internal void Initialize()
		{
			if (_scenes != null)
			{
				for (int i = 0; i < _scenes.Length; ++i)
				{
					SceneInfo sceneInfo = _scenes[i];
					_pathToSceneInfoTable.Add(sceneInfo.Path, sceneInfo);
					_nameToSceneInfoTable.TryAdd(Path.GetFileNameWithoutExtension(sceneInfo.Path), sceneInfo);
				}
			}
		}

		public PackedScene GetPackedSceneByPath(string path)
		{
			TryGetPackedSceneByPath(path, out PackedScene packedScene);

			return packedScene;
		}

		public bool TryGetPackedSceneByPath(string path, out PackedScene scene)
		{
			if (_pathToSceneInfoTable.TryGetValue(path, out SceneInfo sceneInfo))
			{
				scene = sceneInfo.PackedScene;
				return true;
			}

			scene = null;
			return false;
		}

		/// <summary><inheritdoc cref="TryGetPackedSceneByName"/></summary>
		public PackedScene GetPackedSceneByName(string name)
		{
			TryGetPackedSceneByName(name, out PackedScene packedScene);

			return packedScene;
		}

		/// <summary>
		/// Try to get the referenced scene by name. <br />
		/// Warning: if multiple scenes have the same name, only the first one that was added to the list should be returned. 
		/// It is <b><u>strongly advised</u></b> to NOT have multiple scenes with the same name in the scene list.
		/// </summary>
		public bool TryGetPackedSceneByName(string name, out PackedScene scene)
		{
			if (_nameToSceneInfoTable.TryGetValue(name, out SceneInfo sceneInfo))
			{
				scene = sceneInfo.PackedScene;
				return true;
			}
			
			scene = null;
			return false;
		}

		public PackedScene GetPackedSceneByIndex(int index)
		{
			TryGetPackedSceneByIndex(index, out PackedScene packedScene);

			return packedScene;
		}

		public bool TryGetPackedSceneByIndex(int index, out PackedScene scene)
		{
			if (index < 0 || index >= _scenes.Length)
			{
				GD.PushError("scene index is out of bounds!");
				scene = null;
				return false;
			}
			else if (_scenes[index] == null)
			{
				GD.PushError($"scene at specified index:{index} is null!");
				scene = null;
				return false;
			}
			else
				scene = _scenes[index].PackedScene;

			return scene != null;
		}
	}
}
