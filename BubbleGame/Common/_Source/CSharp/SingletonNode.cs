using Godot;

namespace BubbleGame.Common
{
	public partial class SingletonNode<T> : Node where T : Node
	{
		[Export]
		protected bool _destroyOnExitTree = false;

		private static T _instance;
		public static T Instance => _instance;

		public static bool IsInitialized => _instance != null;

		public override void _EnterTree()
		{
			base._EnterTree();

			_instance = this as T;
			Initialize();
		}

		protected virtual void Initialize()
		{
			//
		}

		public override void _ExitTree()
		{
			base._ExitTree();

			if (_destroyOnExitTree)
				_instance = null;
		}
	}
}
