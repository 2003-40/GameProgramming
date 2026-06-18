# Mineral Odyssey - 5-Minute Unity Live Demo Outline

* **演示形式：** 窗口化运行 Unity 编辑器/游戏 Build，一边操作游戏，一边进行全英文解说。
* **核心策略：** 每一个操作动作（Action）都要精准对应一个设计（Design）或技术（Tech）要点。

---

## ⏱️ Live Demo 流程与时间分配

| 阶段 | 游戏内场景/操作位置 | 核心演示与口述重点 | 目标用时 |
| :--- | :--- | :--- | :--- |
| **1. Main Menu & Map Hall** | 主菜单 $\rightarrow$ 地图大厅 | 游戏世界观、核心循环、S型关卡设计、门票机制 | 45秒 |
| **2. Core Mining Loop (Level 1)**| 进入 Level 1 矿区 | 动作体力消耗（Budget）、瓦片破坏、金币转换、反馈系统 | 60秒 |
| **3. Risk & Depth Pressure (Level 2/3)**| 进入 Level 2 或 Level 3 | 怪物碰撞伤害、深度负面状态（减速/静止扣血）、卡牌系统 | 75秒 |
| **4. Upgrades & Robust Architecture** | 结束探索 $\rightarrow$ 升级商店 | 局外养成循环、代码架构（ScriptableObjects / Bootstrap系统） | 60秒 |
| **5. Testing, AI & Professionalism** | 留在游戏界面（或切到代码） | 调试经历、核心反馈改动、资产与 AI 声明、Q&A 准备 | 60秒 |

---

## 🎤 详细演练步骤与实机对齐脚本

### 🕹️ 第一阶段：游戏引入与大厅设计 (0:00 - 0:45)
* **【实机动作 1】** 启动游戏，从 **Main Menu** 点击 Start 进入 **Map Hall**。鼠标在屏幕上晃动，指向 S 型的矿车路线和不同的关卡节点（Level Nodes）。
* **【英文口述内容】**
  > "Hello, welcome to **Mineral Odyssey**, this is a 2D top-down pixel-art mining game built in Unity. As you can see, we starts here in the **Map Hall**, featuring an S-shaped mine-cart route going from Level 1 to Level 3. 
  > 
  > My core design pillar here is structured, paced progression. Level 1 is free, but deeper levels require a **Ticket Cost** in gold. This ensures players can't just skip ahead without engaging with the core gameplay loop."

---

### 🕹️ 第二阶段：核心挖掘循环（实机演示 Level 1）(0:45 - 1:45)
* **【实机动作 2】** 点击进入 **Level 1**。操控角色使用 WASD 移动，面对一个矿石瓦片进行挖掘（点击鼠标）。**让评审看到**右下角的体力条（Stamina Bar）在减少，金币在增加，矿石碎屑在飞散（Particle Pool效果）。
* **【英文口述内容】**
  > "Let’s enter Level 1. Here, the core mechanic is an **action-based stamina system** instead of a real-time countdown. Every single swing is a budget decision. Mining costs stamina based on the ore's hardness and my current tool level.
  > 
  > Technically, this is managed by the `MiningController` and `MiningTile` scripts. Notice the instant visual feedback: when I hit the tile, it triggers a subtle wobble, damage tinting, and particle effects handled by a highly optimized `MiningParticlePool`. Once broken, the drop uses an `ItemMagnet` to smooth out the pickup experience, converting the items directly into saved gold."

---

### 🕹️ 第三阶段：风险控制、深度压力与随机卡牌 (1:45 - 3:00)
* **【实机动作 3】** 手动退出当前 Run（或故意把体力耗尽触发 Run-End）返回大厅，购买门票进入 **Level 2 或 Level 3**。在场景中找到怪物，**故意让怪物撞一下**，展示右下角体力扣除；然后**故意站在原地不动 2-3 秒**，展示环境机制的惩罚。随后在满足条件时触发一次 **Run Card Choice UI**，展示三选一的随机卡牌。
* **【英文口述内容】**
  > "Now, as we move into deeper levels, the game introduces risk and depth pressure. Watch what happens when this lightweight monster hits me: it doesn't reduce health, it damages my **stamina budget** directly, forcing tighter positioning decisions. 
  > 
  > In Level 2, movement is slightly slowed down, and here in Level 3, standing still for too long triggers a **light idle stamina drain** to encourage continuous tactical exploration. 
  > 
  > To break the monotony, I implemented this temporary **Run Card System** managed by `RunCardManager`. Every run presents a random choice of cards—some grant positive buffs like stamina reduction, while others introduce risky, high-reward modifiers."

---

### 🕹️ 第四阶段：局外养成循环与健壮的技术架构 (3:00 - 4:00)
* **【实机动作 4】** 让角色体力归零，触发 **RunEndPanel**，点击返回大厅。打开 **Shop UI Panel**，点击升级工具（Tool Upgrade）或武器（Weapon Upgrade）。
* **【英文口述内容】**
  > "When stamina reaches zero, the run ends, and we see the `RunEndPanel` summarizing our earned earnings. All gold is saved persistently via `PlayerPrefs` and brought back to this **Upgrade Shop**. 
  > 
  > Upgrading my tool reduces mining stamina costs and gates harder ores, successfully closing our progression loop. 
  > 
  > **Project Management & Scope Control:** Originally, I planned a massive crafting and forging system. However, to secure a polished vertical slice, I made a disciplined decision to cut the complex crafting chain and replace it with this streamlined, UI-driven upgrade path. 
  > 
  > **Technical Architecture:** Architecturally, everything is data-driven using Unity `ScriptableObjects`. Furthermore, I wrote robust **Runtime Bootstrap scripts**. Even if manual scene references are missing during intense Unity iteration, these scripts automatically generate and repair the UI linkages at runtime, keeping the demo 100% stable."

---

### 🕹️ 第五阶段：测试反思、资产归属与总结 (4:00 - 5:00)
* **【实机动作 5】** 停留在游戏画面（或者如果你想炫技，可以顺手把 Unity 编辑器切到 `Player.cs` 或 `MiningController.cs` 代码界面停留几秒，证明是你写的代码），然后切回主菜单。
* **【英文口述内容】**
  > "During playtesting and debugging, I encountered a critical issue where decorative assets were blocking UI button clicks. I resolved this by auditing canvas raycast settings and ensuring a consistent global `EventSystem` accompanied my bootstrap logic. Testing also pushed me to centralize gold as the core motivation, directly tying runs to the shop.
  > 
  > **Credits & AI Declaration:** All third-party art and sound assets used are strictly licensed under CC0 or open-source attribution, such as art by AxulArt and Ardonie. AI tools like ChatGPT were used ethically as a technical co-pilot for code structuring and debugging, while all final design control, script compilation, and testing were completely executed by myself.
  > 
  > In conclusion, Mineral Odyssey delivers a highly stable, tightly scoped, and engaging vertical slice. Thank you for your time, and I'm now ready for any live questions or specific code walk-throughs."