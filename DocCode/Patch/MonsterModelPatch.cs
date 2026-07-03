using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Doc.DocCode.Monsters;
using Godot;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Doc.DocCode.Patches;

[HarmonyPatch]
public static class MonsterModelPatch
{
    // 缓存 UpdateBounds 方法
    private static MethodInfo? _updateBoundsMethod;
    private static MethodInfo? _updateBoundsWithStringMethod;

    private static MethodInfo GetUpdateBoundsMethod()
    {
        if (_updateBoundsMethod == null)
        {
            _updateBoundsMethod = AccessTools.Method(typeof(NCreature), "UpdateBounds", new[] { typeof(Node) });
        }
        return _updateBoundsMethod;
    }

    private static MethodInfo GetUpdateBoundsWithStringMethod()
    {
        if (_updateBoundsWithStringMethod == null)
        {
            _updateBoundsWithStringMethod = AccessTools.Method(typeof(NCreature), "UpdateBounds", new[] { typeof(string) });
        }
        return _updateBoundsWithStringMethod;
    }

    // 原有的 Prefix 补丁：拦截 CreateVisuals
    [HarmonyPatch(typeof(MonsterModel), nameof(MonsterModel.CreateVisuals))]
    [HarmonyPrefix]
    public static bool CreateVisualsPrefix(MonsterModel __instance, ref NCreatureVisuals __result)
    {
        if (__instance is ICustomSummon customSummon)
        {
            string scenePath = customSummon.VisualsScenePath;

            if (!string.IsNullOrEmpty(scenePath) && ResourceLoader.Exists(scenePath))
            {
                var scene = GD.Load<PackedScene>(scenePath);
                if (scene != null)
                {
                    try
                    {
                        var instance = scene.Instantiate();
                        if (instance != null)
                        {
                            var visuals = new NCreatureVisuals();

                            if (instance is Node2D node2d)
                            {
                                var children = new List<Node>();
                                foreach (var child in node2d.GetChildren())
                                {
                                    children.Add(child);
                                }

                                foreach (var child in children)
                                {
                                    node2d.RemoveChild(child);
                                    visuals.AddChild(child);
                                    child.Owner = visuals;
                                    child.UniqueNameInOwner = true;
                                }

                                EnsureRequiredNodes(visuals);

                                __result = visuals;
                                MainFile.Logger.Info($"Loaded custom visuals for {__instance.GetType().Name}");
                                return false;
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        MainFile.Logger.Warn($"Failed to load scene: {e.Message}");
                    }
                }
            }

            // 备用方案
            var fallbackVisuals = new NCreatureVisuals();
            CreateFallbackVisuals(fallbackVisuals, customSummon.FallbackTexturePath);
            __result = fallbackVisuals;
            MainFile.Logger.Info($"Using fallback visuals for {__instance.GetType().Name}");
            return false;
        }

        return true;
    }

    // 在 NCreature._Ready() 完成后，使用 await 等待一帧再更新 Bounds
    [HarmonyPatch(typeof(NCreature), nameof(NCreature._Ready))]
    [HarmonyPostfix]
    public static async void NCreatureReadyPostfix(NCreature __instance)
    {
        // 如果是自定义召唤物
        if (__instance.Entity?.Monster is ICustomSummon)
        {
            // 等待两帧，确保所有节点完全初始化
            await __instance.ToSignal(__instance.GetTree(), SceneTree.SignalName.ProcessFrame);
            await __instance.ToSignal(__instance.GetTree(), SceneTree.SignalName.ProcessFrame);

            // 通过反射调用私有的 UpdateBounds 方法
            try
            {
                var method = GetUpdateBoundsMethod();
                if (method != null)
                {
                    method.Invoke(__instance, new object[] { __instance.Visuals });
                    MainFile.Logger.Info($"Re-called UpdateBounds for {__instance.Entity?.Monster?.GetType().Name} via reflection");
                }
                else
                {
                    MainFile.Logger.Warn("Could not find UpdateBounds method");
                }
            }
            catch (System.Exception e)
            {
                MainFile.Logger.Warn($"Failed to call UpdateBounds: {e.Message}");
            }
        }
    }

    private static void EnsureRequiredNodes(NCreatureVisuals visuals)
    {
        // 检查 %Visuals
        var visualsNode = visuals.GetNodeOrNull<Node2D>("%Visuals");
        if (visualsNode == null)
        {
            visualsNode = visuals.GetNodeOrNull<Node2D>("Visuals");
            if (visualsNode != null)
            {
                visualsNode.UniqueNameInOwner = true;
            }
            else
            {
                var body = new Sprite2D { Name = "Visuals" };
                body.UniqueNameInOwner = true;
                visuals.AddChild(body);
                body.Owner = visuals;
            }
        }

        // 检查 %Bounds
        var boundsNode = visuals.GetNodeOrNull<Control>("%Bounds");
        if (boundsNode == null)
        {
            boundsNode = visuals.GetNodeOrNull<Control>("Bounds");
            if (boundsNode != null)
            {
                boundsNode.UniqueNameInOwner = true;
                if (boundsNode.Size == Vector2.Zero)
                {
                    boundsNode.Size = new Vector2(80, 80);
                }
                MainFile.Logger.Info($"Found existing Bounds node with size {boundsNode.Size}");
            }
            else
            {
                var bounds = new Control { Name = "Bounds" };
                bounds.UniqueNameInOwner = true;
                bounds.Size = new Vector2(80, 80);
                visuals.AddChild(bounds);
                bounds.Owner = visuals;
                MainFile.Logger.Info("Created missing Bounds node");
            }
        }
        else
        {
            if (boundsNode.Size == Vector2.Zero)
            {
                boundsNode.Size = new Vector2(80, 80);
            }
            MainFile.Logger.Info($"Found %Bounds node with size {boundsNode.Size}");
        }

        // 检查 %IntentPos
        var intentNode = visuals.GetNodeOrNull<Marker2D>("%IntentPos");
        if (intentNode == null)
        {
            intentNode = visuals.GetNodeOrNull<Marker2D>("IntentPos");
            if (intentNode != null)
            {
                intentNode.UniqueNameInOwner = true;
            }
            else
            {
                var intentPos = new Marker2D { Name = "IntentPos" };
                intentPos.UniqueNameInOwner = true;
                intentPos.Position = new Vector2(0, -60);
                visuals.AddChild(intentPos);
                intentPos.Owner = visuals;
            }
        }

        // 检查 %CenterPos
        var centerNode = visuals.GetNodeOrNull<Marker2D>("%CenterPos");
        if (centerNode == null)
        {
            centerNode = visuals.GetNodeOrNull<Marker2D>("CenterPos");
            if (centerNode != null)
            {
                centerNode.UniqueNameInOwner = true;
            }
            else
            {
                var centerPos = new Marker2D { Name = "CenterPos" };
                centerPos.UniqueNameInOwner = true;
                centerPos.Position = Vector2.Zero;
                visuals.AddChild(centerPos);
                centerPos.Owner = visuals;
            }
        }

        // OrbPos 可选
        var orbNode = visuals.GetNodeOrNull<Marker2D>("%OrbPos");
        if (orbNode == null)
        {
            orbNode = visuals.GetNodeOrNull<Marker2D>("OrbPos");
            if (orbNode != null)
            {
                orbNode.UniqueNameInOwner = true;
            }
            else
            {
                var orbPos = new Marker2D { Name = "OrbPos" };
                orbPos.UniqueNameInOwner = true;
                orbPos.Position = new Vector2(0, 40);
                visuals.AddChild(orbPos);
                orbPos.Owner = visuals;
            }
        }
    }

    private static void CreateFallbackVisuals(NCreatureVisuals visuals, string texturePath = null)
    {
        var visualContainer = new Sprite2D();
        visualContainer.Name = "Visuals";
        visualContainer.UniqueNameInOwner = true;
        visuals.AddChild(visualContainer);
        visualContainer.Owner = visuals;

        Texture2D? texture = null;
        if (!string.IsNullOrEmpty(texturePath) && ResourceLoader.Exists(texturePath))
        {
            texture = GD.Load<Texture2D>(texturePath);
        }

        if (texture != null)
        {
            visualContainer.Texture = texture;
        }

        var bounds = new Control();
        bounds.Name = "Bounds";
        bounds.UniqueNameInOwner = true;
        bounds.Size = new Vector2(80, 80);
        visuals.AddChild(bounds);
        bounds.Owner = visuals;

        var intentPos = new Marker2D();
        intentPos.Name = "IntentPos";
        intentPos.UniqueNameInOwner = true;
        intentPos.Position = new Vector2(0, -60);
        visuals.AddChild(intentPos);
        intentPos.Owner = visuals;

        var centerPos = new Marker2D();
        centerPos.Name = "CenterPos";
        centerPos.UniqueNameInOwner = true;
        centerPos.Position = Vector2.Zero;
        visuals.AddChild(centerPos);
        centerPos.Owner = visuals;

        var orbPos = new Marker2D();
        orbPos.Name = "OrbPos";
        orbPos.UniqueNameInOwner = true;
        orbPos.Position = new Vector2(0, 40);
        visuals.AddChild(orbPos);
        orbPos.Owner = visuals;
    }
}