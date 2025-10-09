// Frontend/app/routes/home.jsx
import { useState } from "react";

export async function loader() {
  return null; // satisfy react-router loader
}

export default function HomePage() {
  // ✅ hooks MUST be inside the component body (not top-level)
  const [message, setMessage] = useState("");
  const [tradeId, setTradeId] = useState("");
  const [userId, setUserId] = useState("2");

  const handleTradeAction = async (id, action) => {
    try {
      const res = await fetch(`/api/trade/${id}/${action}`, {
        method: "POST",
        headers: { "X-User-Id": userId },
      });
      const text = await res.text();
      if (res.ok) {
        setMessage(`${action.toUpperCase()} OK\n${text}`);
      } else {
        setMessage(`${res.status} ${res.statusText}\n${text}`);
      }
    } catch (e) {
      setMessage(String(e));
    }
  };

  return (
    <div className="min-h-screen bg-neutral-900 text-gray-100 flex items-center justify-center p-6">
      {/* phone frame */}
      <div className="relative w-[380px] h-[700px] rounded-2xl border border-black bg-neutral-800 shadow-2xl overflow-hidden">
        {/* top status strip */}
        <div className="absolute left-0 top-0 right-0 h-6" />
        <div className="absolute right-3 top-1 text-[11px] text-amber-400">
          Welcome back, User
        </div>

        {/* content */}
        <div className="p-6 space-y-6">
          {/* logo + title */}
          <div className="flex items-center gap-3">
            <div className="flex h-9 w-9 items-center justify-center rounded-full bg-orange-500/20 ring-1 ring-orange-500/40">
              <span className="text-2xl leading-none">🦔</span>
            </div>
            <div className="text-3xl font-bold tracking-tight">TradeHub</div>
          </div>

          {/* search bar */}
          <div className="mt-2">
            <div className="h-3 w-11/12 rounded-full bg-neutral-600/60" />
          </div>

          {/* grid of tiles */}
          <div className="mt-6 grid grid-cols-4 gap-5 px-2">
            {Array.from({ length: 8 }).map((_, i) => (
              <div
                key={i}
                className="aspect-square rounded-lg bg-gray-400/25 ring-1 ring-white/5"
              />
            ))}
          </div>

          {/* trade actions */}
          <div className="mt-8 text-center">
            <h2 className="text-xl font-semibold text-amber-400 mb-3">
              Trade Actions
            </h2>

            <div className="flex flex-wrap items-center justify-center gap-3 mb-3">
              <label className="text-sm">
                Trade ID:
                <input
                  className="ml-2 rounded-md bg-neutral-900/60 ring-1 ring-white/10 px-3 py-1 outline-none focus:ring-2 focus:ring-amber-500 w-28"
                  value={tradeId}
                  onChange={(e) => setTradeId(e.target.value)}
                  placeholder="e.g. 6"
                />
              </label>
              <label className="text-sm">
                X-User-Id:
                <input
                  className="ml-2 rounded-md bg-neutral-900/60 ring-1 ring-white/10 px-3 py-1 outline-none focus:ring-2 focus:ring-amber-500 w-20"
                  value={userId}
                  onChange={(e) => setUserId(e.target.value)}
                  placeholder="2"
                />
              </label>
            </div>

            <div className="flex justify-center gap-4">
              <button
                onClick={() => handleTradeAction(tradeId, "accept")}
                disabled={!tradeId}
                className={`px-5 py-2 rounded-lg font-semibold transition-colors ${
                  tradeId
                    ? "bg-gray-200 hover:bg-gray-300 text-black"
                    : "bg-gray-300 text-black/50 cursor-not-allowed"
                }`}
              >
                ✅ Accept
              </button>

              <button
                onClick={() => handleTradeAction(tradeId, "reject")}
                disabled={!tradeId}
                className={`px-5 py-2 rounded-lg font-semibold transition-colors ${
                  tradeId
                    ? "bg-gray-200 hover:bg-gray-300 text-black"
                    : "bg-gray-300 text-black/50 cursor-not-allowed"
                }`}
              >
                ❌ Reject
              </button>
            </div>



            {message && (
              <pre className="mt-3 text-sm text-amber-300 whitespace-pre-wrap">
                {message}
              </pre>
            )}
          </div>
        </div>

        {/* footer */}
        <div className="absolute bottom-2 left-0 right-0 text-center text-[11px] text-amber-500">
          2025 Project 2 Team Hedgehog
        </div>
      </div>
    </div>
  );
}
