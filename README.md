# 游戏名称：The Azure Lattice (青金格窗)
**开发版本：** Unity 2022.3 (URP)
**开发者：** Sihan Wang
**项目类型：** 3分钟视觉叙事解谜游戏 (Vertical Slice)

## 1. 游戏愿景 (Game Concept)
《The Azure Lattice》是一款受到《Gorogoa》和《笼中窥梦》启发的视觉解谜小品。游戏以波斯精美的瓷砖建筑为载体，通过“无限缩放”和“拼贴艺术”展现沙漠文明的诗意。玩家将在3分钟的流程中，从一扇格窗深入到一个微缩的宇宙。

## 2. 核心机制 (Core Mechanics)
*   **无缝缩放 (Seamless Zooming):** 点击特定区域（如格窗内部），镜头将平滑切入，揭示隐藏在图案中的新场景。
*   **图案对齐 (Pattern Alignment):** 旋转混乱的瓷砖碎片，当图案完整时，通往下一层空间的路径将会开启。
*   **拼贴叙事 (Collage Storytelling):** 所有的视觉元素均来自公共领域（Public Domain）的波斯细密画与建筑摄影。

## 3. 技术栈 (Technical Stack)
*   **Cinemachine:** 用于实现不同层级间的平滑镜头过渡。
*   **Sprite Masks:** 实现“窗中画”的物理遮挡效果。
*   **LeanTween/DOTween:** 用于处理碎片旋转和 UI 的平滑反馈。
*   **URP 2D Renderer:** 利用 2D 光照增强瓷砖的釉面质感。

## 4. 开发进度 (Roadmap)
- [x] 核心概念确立与素材搜集
- [ ] 原型开发：实现基础缩放逻辑 (In Progress)
- [ ] 关卡设计：三层嵌套空间构建
- [ ] 视觉打磨：粒子效果（风沙）与音效添加
- [ ] 最终测试与性能优化

## 5. 资源致谢 (Credits)
*   [此处填写你提供的图片来源链接，如 Wikimedia Commons/Met Museum]