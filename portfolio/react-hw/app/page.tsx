"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { AuthPanel } from "@/components/AuthPanel";
import { useAuth } from "@/contexts/AuthContext";

export default function HomePage() {
  const { session, loading } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (session) router.replace("/dashboard");
  }, [router, session]);

  if (loading) return <main className="loading-screen">Loading session...</main>;
  if (session) return null;

  return <AuthPanel />;
}
