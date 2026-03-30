# 🌌 Unity 太阳系项目

基于 Unity 2021.3 的太阳系模拟项目，包含地球、月球、太阳等天体的交互展示。

## 📋 项目信息

- **Unity 版本**: 2021.3.45f2c1
- **开发环境**: Windows / Unity Editor
- **仓库地址**: https://github.com/870171594/unity-solar-system

---

## 🚀 快速开始

### 环境要求

- [Unity 2021.3](https://unity.com/releases/editor/archive) 或更高版本
- [Git](https://git-scm.com/downloads) (用于版本控制)

### 克隆项目

```bash
git clone https://github.com/870171594/unity-solar-system.git
```

克隆完成后，用 Unity 打开 `unity-solar-system` 文件夹即可。

> ⚠️ **注意**: 第一次打开项目时，Unity 会自动重新生成 Library 文件夹，可能需要几分钟时间。

---

## 🎮 功能说明

| 功能 | 描述 | 脚本位置 |
|------|------|----------|
| 🎥 摄像机控制 | 策略游戏风格的摄像机操作，类似《无尽的拉格朗日》 | `Assets/Scripts/CameraControl.cs` |
| 🔄 公转控制 | 控制天体公转动画 | `Assets/Scripts/PublicTurn.cs` |
| 🌀 自转控制 | 控制天体自转动画 | `Assets/Scripts/SelfTurn.cs` |
| ⚙️ FPS 设置 | 帧率显示与限制 | `Assets/Scripts/FpsSetting.cs` |
| 🎨 质量设置 | 图形质量切换 | `Assets/Scripts/QualitySetting.cs` |
| 📐 角度设置 | 视角调整 | `Assets/Scripts/AngleSetting.cs` |
| 🖼️ 面板显示 | UI 面板控制 | `Assets/Scripts/PanelShow.cs` |
| 🚪 退出功能 | 应用退出 | `Assets/Scripts/Exit.cs` |

---

## 👥 团队协作指南

### 第一次使用

1. **安装 Git**: 下载 [Git for Windows](https://git-scm.com/download/win)
2. **配置身份** (只需一次):
   ```bash
   git config user.name "你的名字"
   git config user.email "你的邮箱"
   ```
3. **克隆项目**:
   ```bash
   git clone https://github.com/870171594/unity-solar-system.git
   ```

### 日常工作流程

#### 📥 开始工作前
```bash
cd unity-solar-system
git pull
```

#### 📤 完成工作后
```bash
git add .
git commit -m "描述你的修改"
git push
```

> 💡 **提示**: 提交信息要写清楚，例如："添加了FPS设置UI" 而不是 "update"

---

## ⚠️ 重要注意事项

### Git 使用规范

| ✅ 推荐做法 | ❌ 避免做法 |
|------------|------------|
| 每次工作前先 `git pull` | 直接修改不更新 |
| 写清楚的提交信息 | 写 "fix"、"update" |
| 遇到冲突找人商量 | 自己强制解决 |
| 脚本文件可以多人协作 | 场景文件尽量一人负责 |

### 冲突处理

当多人同时修改同一文件时会出现冲突：

```bash
git pull  # 会提示有冲突
```

冲突标记示例：
```
<<<<<<< HEAD
你的修改
=======
别人的修改
>>>>>>> origin/main
```

**解决方法**:
1. 找负责人商量保留哪部分
2. 删除冲突标记符号
3. `git add` + `git commit`

### 文件说明

| 文件/文件夹 | 是否提交 | 说明 |
|-------------|----------|------|
| `Assets/` | ✅ | 项目资源（脚本、场景、材质等） |
| `ProjectSettings/` | ✅ | 项目设置 |
| `Library/` | ❌ | Unity 缓存（自动生成，已忽略） |
| `Temp/` | ❌ | 临时文件（已忽略） |
| `.gitignore` | ✅ | Git 忽略配置 |

---

## 📚 常用命令速查

```bash
# 查看当前状态
git status

# 查看提交历史
git log

# 查看具体修改内容
git diff

# 撤销某个文件的修改
git checkout -- 文件名

# 放弃所有修改
git reset --hard HEAD
```

---

## 🆘 遇到问题？

### 常见问题

**Q: 提示 "Please tell me who you are"？**
```bash
git config user.name "你的名字"
git config user.email "你的邮箱"
```

**Q: push 时要求输入密码？**
输入你的 GitHub 账号密码即可。建议配置 SSH 密钥避免每次输入。

**Q: Unity 打开后 Library 很大，要提交吗？**
不需要！Library 已在 .gitignore 中忽略，每个人本地都会自动生成。

**Q: 不小心 push 了错误的代码怎么办？**
联系负责人，不要自己尝试强制推送！

---

## 📖 更多帮助

- 📄 详细的 [Git 使用手册](./Git使用手册.html) （在浏览器中打开查看）
- 💬 有问题联系项目负责人

---

## 📝 更新日志

- **2025-03-30**: 初始提交，项目结构搭建完成
  - 摄像机控制系统
  - 太阳系场景（地球、月球、太阳）
  - UI 设置面板

---

<div align="center">

**🌟 Happy Coding! 🌟**

</div>
