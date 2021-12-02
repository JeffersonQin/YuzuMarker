# YuzuMarker

[WIP] 次世代漫画汉化工具。本项目主要想要实现漫画汉化过程中多人协作，让工作流程简单化、系统化。

# 架构

<div align="center">
	<img src="./assets/framework.png" width=400>
</div>

# 功能

预期实现功能：

🚧: WIP, 🕒: Not yet started, ⚠: Warning, ✅: Finished, ❌: Abandoned

* 🚧 专用文件格式
  * 🚧 一般格式
    * ✅ 图片资源
    * ✅ PSD 资源
    * ✅ 录入文本
    * ✅ 涂白设置
    * 🚧 嵌字设置
    * ✅ IO 框架
      * 🚧 文件清理
  * 🚧 资源文件加密支持
* 🚧 Photoshop 自动导出
  * ✅ 兼容层
  * ❌  COM 方案 [Deprecated, 停止开发]
  * 🚧 [CEP 方案](https://github.com/JeffersonQin/YuzuMarker.Photoshop)
  * 🕒 UXP 方案 (等待这个方案成熟后再开工)
* ✅ 翻译标注部分
* 🚧 文字录入部分
  * ✅ 文字录入界面
  * 🚧 特殊符号 / 罗马字工具箱
* 🚧 涂白部分
  * ✅ 框选界面
  * 🚧 模型结果选择界面
  * 🚧 文字块自动识别
    * ✅ FindContours
    * 🚧 RCNN
  * 🚧 多种涂白方式
    * 🚧 自动识别颜色
    * 🚧 Impainting
* 🚧 嵌字部分
  * 🚧 嵌字界面
  * 🚧 [字体自动识别](https://github.com/JeffersonQin/YuzuMarker.FontDetection)
  * 🚧 [字体布局半自动化](https://github.com/JeffersonQin/YuzuMarker.TextAutoLayout)
* ✅ 辅助框选工具
  * ✅ 多模式支持: New, Add, Subtract, Intersect
  * ✅ 多形状支持: Lasso, Rect, Oval
* ✅ 撤回重做栈
  * ✅ 撤回重做基本抽象
  * ✅ 链式撤回重做
  * ✅ 撤回重做管理
  * ✅ 对使用工具的撤回重做支持
  * ✅ 对选择内容的撤回重做支持
* ✅ 触摸屏手势支持
* ✅ 触控板手势支持

# 关于 Photoshop 内的操作规范

对于图片的操作，我们主要进行两种操作：

* 涂白
* 嵌字

由此可以引出我们对于图层的排布要求（全部都是自动生成的）：

* 原图图层置于最底部
* 所有自动涂白的背景位于同一个图层内 (为了节省开销)
* 每个自定义涂白分别有各自的图层 (全部都为原图图层复制后生成的蒙版)
* 每个嵌入文本分别有各自的文本图层

下面是结构：

```
(顶)
* CustomTextItems                        # 自定义嵌字图层组
  * TextGroup-p                          # 区域 p 的自定义嵌字图层组
	* Text                               # 区域 p 的自定义嵌字图层组的文字
	* ... (Unmanaged Custom layers)      # 区域 p 的自定义嵌字图层组的其他图层
  * TextGroup-q                          # 区域 q 的自定义嵌字图层组
	* Text                               # 区域 q 的自定义嵌字图层组的文字
	* ... (Unmanaged Custom layers)      # 区域 q 的自定义嵌字图层组的其他图层
  * ...
* AutoTextItems                          # 自动导出的嵌字图层组
  * Text-a                               # 区域 a 的自动导出的嵌字
  * Text-b                               # 区域 b 的自动导出的嵌字
  * ...
* AutoBackground                         # 自动导出的背景图层组
  * Impainting                           # 自动导出的背景修复图层组
    * Impainting-r                       # 区域 r 的自动导出的背景修复图层
	* Impainting-s                       # 区域 s 的自动导出的背景修复图层
	* ...
  * Color                                # 自动导出的纯色填充图层组
    * ColorExport                        # 自动导出的纯色填充图层
* CustomBackground                       # 自定义背景图层组
  * Background-c                         # 区域 c 的自定义背景图层组
	* ...
  * Background-d                         # 区域 d 的自定义背景图层组
  	* ...
  * ...
* Background                             # 背景图层
(底)
```

注意:

* 某些图层组只包含一个图层，原因是这样可以固定图层组间的顺序。
* 嵌字图层和涂白图层不同。由 Photoshop 的文档我们可知，文字图层是一种图层类型，那也就注定了我们需要对于每一组文字分别建立对应的文字图层，所以就算是自定义文字图层，只需在项目中标记，无需在 Photoshop 中做出记号亦可处理。
* `自动导出背景图层组` 在 `自定义背景图层组` 之上的原因是，这样蒙版的区域可以有更大的容错，无需顾虑。
* 只能保证 `背景图层 => 自定义背景图层组 => 自动导出背景图层组 => 自动导出嵌字图层 => 自定义嵌字图层` 这样的堆叠顺序，不能保证图层组内部的顺序。

# 关于文件格式的设计

```
<project-name>                            # 项目文件夹
├── <project-name>.yuzu                   # 项目文件
├── Images                                # 图片文件夹
|   ├── <image-file-1>                    # 图片 1
|   ├── <image-file-2>                    # 图片 2
|   ├── ...
|   └── <image-file-n>                    # 图片 n
├── PSD                                   # PS 文件夹
|   ├── <image-file-1>.psd                # 图片 1 对应的 .psd
|   ├── <image-file-2>.psd                # 图片 2 对应的 .psd
|   ├── ...
|   └── <image-file-n>.psd                # 图片 n 对应的 .psd
└── Notations                             # 简单文本标注文件夹
    ├── <image-file-1>                    # 图片 1 对应的标注文件夹
    |   ├── index.json                    # 图片 1 标注时间戳索引
    |   ├── <timestamp-1>-simple.json     # 图片 1 的 第1个普通标注
    |   ├── <timestamp-1>-<type>.json     # 图片 1 的 第1个其他种类的标注
    |   ├── ...
    |   ├── <timestamp-2>-simple.json     # 图片 1 的 第2个普通标注
    |   ├── <timestamp-2>-<type>.json     # 图片 1 的 第2个其他种类的标注
    |   ├── ...
    |   ├── <timestamp-m>-simple.json     # 图片 1 的 第m个普通标注
    |   ├── <timestamp-m>-<type>.json     # 图片 1 的 第m个其他种类的标注
    |   └── ...
    ├── <image-file-2>                    # 下同
    |   ├── index.json                    # 图片 2 标注时间戳索引
    |   ├── <timestamp-1>-simple.json
    |   ├── <timestamp-1>-<type>.json
    |   ├── ...
    |   ├── <timestamp-2>-simple.json
    |   ├── <timestamp-2>-<type>.json
    |   ├── ...
    |   ├── <timestamp-m>-simple.json
    |   ├── <timestamp-m>-<type>.json
    |   └── ...
    ├── ...
    └── <image-file-n>
        ├── index.json
        ├── <timestamp-1>-simple.json
        ├── <timestamp-1>-<type>.json
        ├── ...
        ├── <timestamp-2>-simple.json
        ├── <timestamp-2>-<type>.json
        ├── ...
        ├── <timestamp-m>-simple.json
        ├── <timestamp-m>-<type>.json
        └── ...
```

# 关于多人协同设计

使用 git 进行多人协同，考虑到汉化组的成员可能完全不了解 git 原理与思想，所以我们将 git 工作流进行简化：
* 只有一个 master branch
* add 和 commit 保证同时进行
* 每次 push 前保证先 pull
* 文件格式的设计保证了一个文件出现 conflict 时只针对一个问题
* 做一个解决 conflict 的 UI 界面

# 关于加密格式

对于项目文件本身，不打算，也永远不打算做加密系统，徒增代码量。理由很简单，如果要加密，为什么不打个包直接加密。

然而，对于多人协同时，由于涉及到 DMCA，我们会使用加密，特别是图片文件，这种本身就是不能被 diff 的格式，加密一下在 git 工作流上也并没有什么损失。

综上，在 git 系统中，二进制文件可以选择加密，我们会将文件夹进行如下转化：

```
Images => Images-Encrypted
PSD    => PSD-Encrypted
```

此外，我们还会添加配置文件：

```
.yuzugitsettings  # 记录加密设置
.password         # 记录密码
```

并添加如下的 `.gitignore`:

```
Images/
PSD/
.password
```

# LICENSE

对于软件本身，我们使用 MIT License 以及以下追加条款进行授权 (见 [LICENSE](./LICENSE) 文件)：

* 本软件及其副本和衍生软件仅可以用于创建和分发免费或非盈利的衍生品，或由购买商业许可证的公司或个人提供的盈利专有衍生品；
* 上述限制条款应包含在软件的所有修改或者未经修改的副本中。

如果上述条款不能满足您的需求，请[和我联系](mailto://1247006353@qq.com)，讨论商业许可证的相关问题。

注意：
* 由于整个汉化社区的非盈利性质，以及社区对于盈利性汉化活动的态度，促成了上述条款。上述条款与 Open Source Definition 的第六条 `No Discrimination Against Fields of Endeavor` 相冲突，所以并不构成正真意义上的开源，只是在一定条件下公开源代码、允许使用。

# EULA

* 使用本软件的风险需要由使用者自行承担，如果有任何违反相关国家或地方法律的行为，与作者无关；
* 本软件仅可以用作学习、教育、交流用途。

# Disclaimer

* 使用本软件的风险需要由使用者自行承担，如果有任何违反相关国家或地方法律的行为，与作者无关；
* 本软件适用于学习、教育、交流用途，适用于 Jetbrains Educational License 的条款。商业许可证授权的版本的开发与 JetBrains 系列 IDE 无实质性关联，为使用 Visual Studio Community 个人独立开发完成，有独立的代码库，遵守了 Visual Studio Community 的 EULA 条款。

2021.11.29 Update: JetBrains 针对本 OSS 项目已经提供了免费许可证。

# 感谢

特别感谢@透明声彩汉化组

<img src="./assets/group.png" alt="透明声彩" width=200>

感谢 Jetbrains 提供的教育许可证、OSS 许可证

<img src="./assets/jb_beam.svg" alt="JetBrains Logo (Main) logo." width=200><img src="./assets/PyCharm_icon.svg" alt="PyCharm logo." width=200><img src="./assets/Rider_icon.svg" alt="Rider logo." width=200>
