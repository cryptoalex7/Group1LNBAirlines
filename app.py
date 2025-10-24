from fastapi import FastAPI, Header, HTTPException
from pydantic import BaseModel
import os, httpx

OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")
APP_TOKEN = os.getenv("APP_TOKEN")  # optional

if not OPENAI_API_KEY:
    raise RuntimeError("OPENAI_API_KEY not set")

app = FastAPI()

class ChatReq(BaseModel):
    message: str
    userId: str | None = None

@app.post("/chat")
async def chat(req: ChatReq, x_app_token: str | None = Header(default=None, alias="X-App-Token")):
    if APP_TOKEN and x_app_token != APP_TOKEN:
        raise HTTPException(status_code=401, detail="Unauthorized")

    payload = {
        "model": "gpt-4o-mini",
        "messages": [
            {"role": "system", "content": "You are an airline scheduling assistant. Be concise; ask for shift IDs when needed."},
            {"role": "user", "content": req.message}
        ]
    }

    headers = {"Authorization": f"Bearer {OPENAI_API_KEY}"}
    async with httpx.AsyncClient(timeout=60.0) as client:
        r = await client.post("https://api.openai.com/v1/chat/completions",
                              json=payload, headers=headers)
        if r.status_code >= 300:
            raise HTTPException(status_code=502, detail=f"OpenAI error {r.status_code}: {r.text}")
        data = r.json()
        return {"text": data["choices"][0]["message"]["content"]}
