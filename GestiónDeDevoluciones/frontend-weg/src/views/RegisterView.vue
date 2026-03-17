<script setup lang="ts">
import { ref, reactive } from "vue";
import { useRouter } from "vue-router";
import { Loader2 } from "lucide-vue-next";
import api from "../services/api";

const enrutador = useRouter();
const cargando = ref(false);

const datosFormulario = reactive({
  nombreUsuario: "",
  email: "",
  contrasena: "",
  rolId: 2,
});

const erroresCampo = reactive({
  nombreUsuario: "",
  email: "",
  contrasena: "",
});

const errorGeneral = ref("");
const mensajeExito = ref("");

const limpiarErrores = () => {
  erroresCampo.nombreUsuario = "";
  erroresCampo.email = "";
  erroresCampo.contrasena = "";
  errorGeneral.value = "";
  mensajeExito.value = "";
};

const manejarRegistro = async () => {
  limpiarErrores();
  let tieneError = false;

  if (!datosFormulario.nombreUsuario.trim()) {
    erroresCampo.nombreUsuario = "Ingresá tu nombre y apellido.";
    tieneError = true;
  }
  
  const regexEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!datosFormulario.email.trim()) {
    erroresCampo.email = "Ingresá tu email corporativo.";
    tieneError = true;
  } else if (!regexEmail.test(datosFormulario.email)) {
    erroresCampo.email = "El formato del email no es válido.";
    tieneError = true;
  }

  if (datosFormulario.contrasena.length < 6) {
    erroresCampo.contrasena = "La contraseña debe tener al menos 6 caracteres.";
    tieneError = true;
  }

  if (tieneError) return;

  cargando.value = true;
  try {
    await api.post("/api/Auth/register", {
      nombreUsuario: datosFormulario.nombreUsuario,
      email: datosFormulario.email,
      password: datosFormulario.contrasena,
      rolId: datosFormulario.rolId,
    });

    mensajeExito.value = "Registro exitoso. Serás redirigido al inicio de sesión...";
    setTimeout(() => {
      enrutador.push("/login");
    }, 2500);
  } catch (err: any) {
    const mensaje = err.response?.data?.message || "";
    if (mensaje.toLowerCase().includes("email ya está registrado")) {
      erroresCampo.email = "Este email ya se encuentra registrado.";
    } else {
      errorGeneral.value = mensaje || "No se pudo completar el registro. Verificá los datos ingresados.";
    }
  } finally {
    cargando.value = false;
  }
};
</script>

<template>
  <div class="min-h-screen bg-white flex">
    <div class="hidden lg:flex lg:w-2/5 bg-[#003b5c] flex-col justify-between p-14">
      <div>
        <div class="flex items-center gap-3 mb-20">
          <div class="h-8 w-8 rounded-lg bg-white/20 flex items-center justify-center">
            <svg viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2.5" class="h-4 w-4">
              <path d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5"/>
            </svg>
          </div>
          <span class="text-white font-black text-sm tracking-widest uppercase">WEG S.A.</span>
        </div>

        <div>
          <h1 class="text-white text-4xl font-black leading-tight tracking-tight mb-6">
            Bienvenido<br/>al equipo<br/>WEG.
          </h1>
          <p class="text-blue-200/70 text-sm leading-relaxed">
            Todo lo que necesitás para registrar y seguir una devolución, en un solo lugar.
          </p>
        </div>
      </div>

      <div class="border-t border-white/10 pt-8">
        <p class="text-white/30 text-[10px] uppercase tracking-widest font-bold">WEG Equipamientos Eléctricos S.A.</p>
        <p class="text-white/20 text-[10px] mt-1">Uso exclusivo del personal interno</p>
      </div>
    </div>

    <div class="flex-1 flex flex-col items-center justify-center p-8 lg:p-16">
      <div class="w-full max-w-sm">

        <div class="lg:hidden mb-10 text-center">
          <p class="text-[10px] font-bold uppercase tracking-widest text-slate-400">WEG Equipamientos Eléctricos S.A.</p>
        </div>

        <div class="mb-10">
          <h2 class="text-2xl font-black text-slate-900 tracking-tight mb-1">Crear cuenta</h2>
          <p class="text-sm text-slate-400">Completá los datos para registrarte</p>
        </div>

        <div
          v-if="mensajeExito"
          class="mb-6 flex items-start gap-3 rounded-xl border border-emerald-100 bg-emerald-50 px-4 py-3 animate-in fade-in"
        >
          <svg class="h-5 w-5 text-emerald-500 shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <p class="text-xs font-medium text-emerald-800">{{ mensajeExito }}</p>
        </div>

        <div
          v-if="errorGeneral"
          class="mb-6 flex items-start gap-3 rounded-xl border border-red-100 bg-red-50 px-4 py-3"
        >
          <svg class="h-4 w-4 text-red-500 mt-0.5 shrink-0" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
            <circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/>
          </svg>
          <p class="text-xs font-medium text-red-700">{{ errorGeneral }}</p>
        </div>

        <form @submit.prevent="manejarRegistro" novalidate class="space-y-5">
          
          <div>
            <label class="block text-xs font-bold text-slate-600 mb-2">Nombre y Apellido</label>
            <input
              v-model="datosFormulario.nombreUsuario"
              type="text"
              placeholder="Juan Pérez"
              @input="erroresCampo.nombreUsuario = ''"
              :class="['w-full rounded-xl border bg-slate-50 px-4 py-3 text-sm text-slate-900 placeholder-slate-300 outline-none transition focus:bg-white focus:ring-2', erroresCampo.nombreUsuario ? 'border-red-300 focus:border-red-500 focus:ring-red-500/10' : 'border-slate-200 focus:border-[#003b5c] focus:ring-[#003b5c]/10']"
            />
            <p v-if="erroresCampo.nombreUsuario" class="mt-1.5 text-xs font-bold text-red-500 animate-in fade-in slide-in-from-top-1">
              {{ erroresCampo.nombreUsuario }}
            </p>
          </div>

          <div>
            <label class="block text-xs font-bold text-slate-600 mb-2">Email</label>
            <input
              v-model="datosFormulario.email"
              type="email"
              placeholder="juan.perez@weg.com"
              @input="erroresCampo.email = ''"
              :class="['w-full rounded-xl border bg-slate-50 px-4 py-3 text-sm text-slate-900 placeholder-slate-300 outline-none transition focus:bg-white focus:ring-2', erroresCampo.email ? 'border-red-300 focus:border-red-500 focus:ring-red-500/10' : 'border-slate-200 focus:border-[#003b5c] focus:ring-[#003b5c]/10']"
            />
            <p v-if="erroresCampo.email" class="mt-1.5 text-xs font-bold text-red-500 animate-in fade-in slide-in-from-top-1">
              {{ erroresCampo.email }}
            </p>
          </div>

          <div>
            <label class="block text-xs font-bold text-slate-600 mb-2 whitespace-nowrap">
              Contraseña <span class="font-normal text-slate-400 ml-1">(mínimo 6 caracteres)</span>
            </label>
            <input
              v-model="datosFormulario.contrasena"
              type="password"
              placeholder="••••••••"
              @input="erroresCampo.contrasena = ''"
              :class="['w-full rounded-xl border bg-slate-50 px-4 py-3 text-sm text-slate-900 placeholder-slate-300 outline-none transition focus:bg-white focus:ring-2', erroresCampo.contrasena ? 'border-red-300 focus:border-red-500 focus:ring-red-500/10' : 'border-slate-200 focus:border-[#003b5c] focus:ring-[#003b5c]/10']"
            />
            <p v-if="erroresCampo.contrasena" class="mt-1.5 text-xs font-bold text-red-500 animate-in fade-in slide-in-from-top-1">
              {{ erroresCampo.contrasena }}
            </p>
          </div>

          <button
            type="submit"
            :disabled="cargando"
            class="mt-2 w-full flex items-center justify-center gap-2 rounded-xl bg-[#003b5c] py-3.5 text-sm font-bold text-white transition hover:bg-[#004a73] active:scale-[0.98] disabled:opacity-60"
          >
            <Loader2 v-if="cargando" class="h-4 w-4 animate-spin" />
            <span>{{ cargando ? "Registrando..." : "Crear cuenta" }}</span>
          </button>
        </form>

        <p class="mt-8 text-center text-xs text-slate-400">
          ¿Ya tenés cuenta?
          <router-link to="/login" class="font-bold text-[#003b5c] hover:underline ml-1">Iniciá sesión</router-link>
        </p>
      </div>
    </div>
  </div>
</template>
