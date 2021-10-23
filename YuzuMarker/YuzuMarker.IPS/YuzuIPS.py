from fastapi import FastAPI
import uvicorn

host = "127.0.0.1"
port = 1029

app = FastAPI()


if __name__ == '__main__':
    uvicorn.run(app, host=host, port=port)
