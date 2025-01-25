using Godot;

namespace BubbleGame.Common.SceneManagement
{
	[GlobalClass]
	public partial class SceneInfo : Resource
	{
		[Export]
		public PackedScene PackedScene { get; set; }

		public SceneInfo() { }

		public SceneInfo(PackedScene packedScene)
		{
			PackedScene = packedScene;
		}

		public SceneInfo(string scenePath)
		{
			PackedScene = ResourceLoader.Load<PackedScene>(scenePath);
		}

		public string Name
		{
			get
			{
				if (PackedScene == null)
					return null;

				string resourcePath = PackedScene.ResourcePath;

				if (resourcePath == null)
					return null;

				return System.IO.Path.GetFileNameWithoutExtension(resourcePath);
			}
		}

		/// <summary>
		/// The path of the referenced scene. <br />
		/// Note: Use this property instead of the ResourcePath property which would give you the path to this resource 
		/// and not the path to the referenced scene.
		/// </summary>
		public string Path
		{
			get
			{
				if (PackedScene == null)
					return null;

				return PackedScene.ResourcePath;
			}
		}
	}
}
