// Frontend/app/routes/user-profile.jsx
export async function loader() {
  // In the future, this can call your API (e.g. GET /api/users/2)
  return null;
}

export default function UserProfilePage() {
  return (
    <div className="min-h-screen bg-neutral-900 text-gray-100 flex items-center justify-center p-6">
      {/* phone frame */}
      <div className="relative w-[380px] h-[700px] rounded-2xl border border-black bg-neutral-800 shadow-2xl overflow-hidden">
        {/* top strip */}
        <div className="absolute right-3 top-1 text-[11px] text-amber-400">
          Welcome back, User
        </div>

        {/* content */}
        <div className="p-6">
          <h1 className="text-4xl font-extrabold tracking-tight mb-6">User Profile</h1>

          <div className="space-y-4 max-w-[320px]">
            <Field label="Username:">
              <input
                defaultValue="JDonator"
                className="w-full rounded-md bg-neutral-900/60 ring-1 ring-white/10 px-3 py-2 outline-none focus:ring-2 focus:ring-amber-500"
              />
            </Field>

            <Field label="Email:">
              <input
                type="email"
                placeholder="user@example.com"
                className="w-full rounded-md bg-neutral-900/60 ring-1 ring-white/10 px-3 py-2 outline-none focus:ring-2 focus:ring-amber-500"
              />
            </Field>

            <Field label="Description:">
              <textarea
                rows={3}
                placeholder="About me..."
                className="w-full rounded-md bg-neutral-900/60 ring-1 ring-white/10 px-3 py-2 outline-none focus:ring-2 focus:ring-amber-500"
              />
            </Field>

            <Field label="Change Password:">
              <input
                type="password"
                placeholder="New password"
                className="w-full rounded-md bg-neutral-900/60 ring-1 ring-white/10 px-3 py-2 outline-none focus:ring-2 focus:ring-amber-500"
              />
            </Field>

            <div className="pt-2">
              <button
                className="rounded-md bg-red-600 px-4 py-2 font-semibold text-white shadow hover:bg-red-500 focus:outline-none focus:ring-2 focus:ring-red-400"
              >
                Delete User
              </button>
            </div>
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

function Field({ label, children }) {
  return (
    <label className="block">
      <div className="text-lg font-semibold mb-1">{label}</div>
      {children}
    </label>
  );
}
