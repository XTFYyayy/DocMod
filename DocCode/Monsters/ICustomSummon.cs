namespace Doc.DocCode.Monsters;

/// <summary>
/// 标记接口：用于标识自定义召唤物，使用自定义视觉场景
/// </summary>
public interface ICustomSummon
{
    /// <summary>
    /// 自定义视觉场景路径
    /// </summary>
    string VisualsScenePath { get; }

    /// <summary>
    /// 备用纹理路径（当场景加载失败时使用）
    /// </summary>
    string FallbackTexturePath { get; }
}