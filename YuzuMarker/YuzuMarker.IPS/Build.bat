chcp 65001

:: 判断 venv 是否存在, 不存在则建立
:: 需要将 python 修改为目标解释器 python.exe 的路径
if not exist venv (
	python -m venv venv
)

:: 进入 venv
call venv\Scripts\activate.bat

:: 升级 pip
python -m pip install --upgrade pip

:: 安装依赖
pip install -r requirements.txt --index-url https://mirrors.aliyun.com/pypi/simple

:: pyinstaller 打包
.\venv\Scripts\pyinstaller.exe -D -y --distpath %1 YuzuIPS.py