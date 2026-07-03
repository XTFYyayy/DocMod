using Godot;

namespace Doc.Nodes
{
	public partial class SummonVisuals : Node2D
	{
		// 保持继承 Node2D，编辑器可以正常加载

		public void SetupVisuals(string skinPath, float scale = 1f)
		{
			// 加载并设置皮肤
		}

		public void SetupVisuals(Texture2D texture, float scale = 1f)
		{
			// 使用纹理设置外观
		}
	}
}
