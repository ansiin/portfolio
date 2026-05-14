"use client";

import { AuthProvider } from "@/contexts/AuthContext";
import { TodoProvider } from "@/contexts/TodoContext";

export function Providers({ children }: { children: React.ReactNode }) {
  return (
    <AuthProvider>
      <TodoProvider>{children}</TodoProvider>
    </AuthProvider>
  );
}
