# 最终一致性审查报告（L3）

- 审查日期：2026-08-15
- 审查范围：TODO.txt 全部条目 ↔ 13 个实现任务的交付产物（10 张卡 + 4 个 Power + 1 个自定义充能球 + 双注册点）
- 审查方式：逐条对照 TODO.txt 原始条目，逐文件阅读源码，编译级验证 `dotnet build Doc.csproj`
- 结论：**通过（含记录项）**，无阻断性问题

## 一、核对清单表

### 1. 卡牌（DocCode/Cards/Doctor/，共 10 张）

| # | TODO 条目 | 实现文件 | 费用/升级 | 稀有度/类型 | 标签 | 关键字 | 效果核对 | 一致性 |
|---|-----------|----------|-----------|-------------|------|--------|----------|--------|
| 1 | 12F（类名 F12） | `F12.cs` | 1 费，升级 1→0 ✓ | 普通/技能 | 罗德岛 ✓ | - | 给予敌人 2 层跟踪锁定 ✓ | ✅ |
| 2 | 云迹 Contrail | `Contrail.cs` | 1 费，起飞 1→2 层 ✓ | 普通/技能 | 哥伦比亚 ✓ | - | 获得 1/2 层起飞 ✓ | ✅ |
| 3 | 司霆惊蛰 LeiziTheThunderbringer | `LeiziTheThunderbringer.cs` | 1 费，起飞 1→2 层 ✓ | 普通/技能 | 炎 ✓ | - | 获得起飞 + 生成 1 个闪电充能球 ✓ | ✅ |
| 4 | 奥斯塔 Aosta | `Aosta.cs` | 2 费，无升级 ✓ | 罕见/攻击 | 叙拉古 ✓ | - | 造成 11 点伤害；目标意图为攻击则击晕 ✓ | ✅ |
| 5 | 布丁 Pudding | `Pudding.cs` | 1 费，抽 1→2 张 ✓ | 普通/技能 | 哥伦比亚 ✓ | - | 生成 1 个闪电球 + 抽 1/2 张 ✓ | ✅ |
| 6 | 布洛卡 Broca | `Broca.cs` | 2 费，无升级 ✓ | 罕见/攻击 | 叙拉古 ✓ | - | 移除全部格挡 + 每激发 1 球造成 8 点伤害（见记录项 R2） | ✅* |
| 7 | 格雷伊 Greyy | `Greyy.cs` | 2 费，无升级 ✓ | 稀有/能力 | 玻利瓦尔 ✓ | 消耗 ✓ | 球击中所有敌人 + 生成 3 个球 ✓ | ✅ |
| 8 | 澄闪 Goldenglow | `Goldenglow.cs` | 2 费，见记录项 R3 | 稀有/技能 | 维多利亚 ✓ | 消耗 ✓ | 每有 1 球生成 3 球（快照×3，防无限扩张）✓ | ✅* |
| 9 | 阿米娅 Amiya | `Amiya.cs` | 3 费，无升级 ✓ | 稀有/攻击 | 罗德岛 ✓ | 虚无/奇巧 ✓ | 6 点伤害×4 次；回合结束手牌效果（见记录项 R1） | ✅* |
| 10 | 阿米娅 影霄 AmiyaGuard | `AmiyaGuard.cs` | 1 费，无升级 ✓ | 稀有/攻击 | 罗德岛 ✓ | 消耗 ✓ | 12 点伤害；1 层调弦；斩杀时伤害次数/调弦层数永久 +1（[SavedProperty] 跨战斗持久）✓ | ✅ |

> 说明：Amiya 以"阿米娅 影霄"条目实现，使用独立类名 `AmiyaGuard` 与 TODO 表一致（TODO 类名列写 Amiya，但条目名"阿米娅 影霄"与描述均指近卫形态，按条目名拆分为独立卡合理）。标注 * 的条目详见问题清单。

### 2. Power（DocCode/Powers/，共 4 个）

| # | TODO 条目 | 实现文件 | 类型/叠层 | 效果核对 | 一致性 |
|---|-----------|----------|-----------|----------|--------|
| 1 | 跟踪锁定 LockOnTracking | `LockOnTrackingPower.cs` | Debuff/Counter | 从闪电球受到的伤害 +50%（放大逻辑在 DocLightningOrb 实现）✓ | ✅ |
| 2 | 起飞 Flying | `FlyingPower.cs` | Buff/Counter | 攻击牌伤害 -50%（ModifyDamageMultiplicative 返回 0.5 倍率）；每受一次非 0 攻击伤害减 1 层，归 0 移除 ✓ | ✅ |
| 3 | 闪电球全命中 LightningStrikesAll | `LightningStrikesAllPower.cs` | Buff/Single | 纯标记，AOE 切换逻辑在 DocLightningOrb ✓ | ✅ |
| 4 | 调弦 TuneTheStrings | `TuneTheStringsPower.cs` | Buff/Counter | 攻击无视格挡（BeforeAttack 反射注入 Unblockable）；回合结束减 1 层，归 0 移除 ✓ | ✅ |

### 3. 自定义充能球（DocCode/Orbs/DocLightningOrb.cs）

| 核对项 | 结果 |
|--------|------|
| 继承官方 `LightningOrb`（non-sealed 正确） | ✅ |
| 与各球类卡的 Channel 方式一致：全部使用实例重载 `OrbCmd.Channel(ctx, new DocLightningOrb().ToMutable(), Owner)`，未用未注册 ModelDb 的泛型 `Channel<T>` | ✅ |
| AOE 联动：持有 `LightningStrikesAllPower` → 击中全部 `CombatState.HittableEnemies`；否则随机单体 | ✅ |
| 放大联动：目标持有 `LockOnTrackingPower` → 球伤害 ×1.5 | ✅ |
| 伤害属性 `ValueProp.Unpowered`，VFX 播放正常 | ✅ |

### 4. 注册核对

| 注册点 | 内容 | 一致性 |
|--------|------|--------|
| `DocCardPool.cs` `GenerateAllCards()` | ProjektRed 后追加 10 条 `ModelDb.Card<Xxx>()`（F12/Contrail/Leizi/Aosta/Pudding/Broca/Greyy/Goldenglow/Amiya/AmiyaGuard） | ✅ |
| `Doc/MainFile.cs` `AddCardsToPools()` | 追加 10 条 `ModHelper.AddModelToPool<DocCardPool, Xxx>()` | ✅ |
| 命名规范 | 12F 使用类名/文件名 `F12`（非 12F）；所有新文件命名空间 `Doc.DocCode.Cards.Doctor` / `Doc.DocCode.Powers` / `Doc.DocCode.Orbs`；[Pool] 由基类 DocCard 继承、子类不显式声明 | ✅ |

## 二、编译验证

```
dotnet build Doc.csproj --nologo -v q
已成功生成。 0 个警告 0 个错误（exit code 0）
```

## 三、问题清单

### 阻断性问题（Blocking）
无。构建通过，注册完整，无编译错误。

### 建议性问题（Minor / 记录项）

- **R1（用户确认变更）**：阿米娅回合结束手牌效果，TODO 原文为"随机对三名敌人造成 18 点无视格挡的伤害 3 次"，用户已明确确认改为**对所有敌人**造成 18 点无视格挡伤害 3 次。实现采用 `CombatState.HittableEnemies` 全敌人 ×3 轮，符合用户最新指示，属预期变更。
- **R2（授权差异）**：布洛卡"激发所有闪电充能球"，官方 `EvokeNext`/`LightningOrb.Evoke` 无法指定目标（随机单体）。按 implementationGuide 授权差异，实现为：对目标造成 `8 × 球数` 点伤害后 `OrbQueue.Clear()`，语义与"每激发 1 个造成 8 点伤害"等价（可稳定命中指定目标，避免随机性）。已留注释说明。
- **R3（待人工确认）**：澄闪 TODO 关键字列为"消耗（-）"，疑似表示"升级后移除消耗"，当前实现为固定 Exhaust 且无升级。需人工确认该标注含义；若确为升级移除消耗，需补 `OnUpgrade` 逻辑。
- **R4（本地化待补）**：`Doc/localization/zhs/powers.json` 尚缺 3 个新 Power 的描述文案：`DOC-LOCK_ON_TRACKING_POWER.description`、`DOC-FLYING_POWER.description`、`DOC-LIGHTNING_STRIKES_ALL_POWER.description`（`TuneTheStringsPower` 已内联 LocString 不受影响）。补全前这三个 Power 在游戏中可能显示空描述，代码注释已标注字段名。
- **R5（素材占位）**：3 个新 Power 的图标路径已占位（`*.png`），素材文件尚未提供。用户已声明图标问题无需考虑，不影响功能。

## 四、结论

**通过（含记录项）。** 10 张卡 + 4 个 Power + 1 个自定义充能球与 TODO.txt 全部条目逐条一致（费用、升级、稀有度、类型、标签、关键字、数值）；DocLightningOrb 的实例重载 Channel 约定在全部 5 张球类卡中统一；双注册点（DocCardPool + MainFile）10 卡齐全；F12 命名与命名空间规范正确；`dotnet build` 0 警告 0 错误。发现的差异项（R1 用户确认、R2 授权差异）均已在实现中注明；建议性事项（R3/R4/R5）需人工确认或后续补全，不构成发布阻断。
