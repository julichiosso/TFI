<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import { useRoute, useRouter } from "vue-router";
import Sidebar from "../components/Sidebar.vue";
import api from "../services/api";
import {
  ChevronLeft,
  Building2,
  MapPin,
  Loader2
} from "lucide-vue-next";
import { usarAutenticacion } from "../store/auth";

const ruta = useRoute();
const enrutador = useRouter();
const autenticacion = usarAutenticacion();
const esAdmin = computed(() => {
  const rol = autenticacion.usuario.value?.rol;
  return rol === 'Administrador' || rol === 'admin';
});
const cliente = ref<any>(null);
const cargando = ref(true);

const mostrarAlerta = (mensaje: string) => {
  if (typeof window !== 'undefined') window.alert(mensaje);
};

onMounted(async () => {
  try {
    const respuesta = await api.get(`/api/Cliente/${ruta.params.id}`);
    cliente.value = respuesta.data;
  } catch (err) {
    console.error("Error al obtener detalles del cliente:", err);
  } finally {
    cargando.value = false;
  }
});
</script>

<template>
  <div class="flex min-h-screen bg-[#fcfcfd]">
    <Sidebar />

    <main class="flex-1 p-6 lg:p-12 overflow-y-auto no-scrollbar">
      <div v-if="cargando" class="flex h-[80vh] items-center justify-center">
        <div class="flex flex-col items-center gap-4">
          <Loader2 class="h-10 w-10 animate-spin text-[#005792]" />
          <p class="text-[10px] font-black uppercase tracking-widest text-slate-400">Consultando Base de Datos...</p>
        </div>
      </div>

      <div v-else-if="cliente" class="max-w-6xl mx-auto animate-in fade-in duration-700">
        <div class="mb-10 flex flex-col md:flex-row md:items-center justify-between gap-6">
          <button
            @click="enrutador.back()"
            class="flex items-center gap-2 text-xs font-black uppercase tracking-widest text-slate-400 hover:text-slate-900 transition-colors"
          >
            <ChevronLeft class="h-4 w-4" />
            Volver al Directorio
          </button>
          
          <div class="flex items-center gap-3">
            <button 
              v-if="esAdmin"
              @click="enrutador.push(`/clients/edit/${cliente.clienteId}`)"
              class="rounded-xl border border-slate-200 bg-white px-5 py-3 text-[10px] font-black uppercase tracking-widest text-slate-600 hover:bg-slate-50 hover:text-slate-900 transition-all shadow-sm"
            >
              Editar Perfil
            </button>
            <button 
              @click="mostrarAlerta('Generando reporte PDF...')"
              class="rounded-xl bg-slate-900 px-5 py-3 text-[10px] font-black uppercase tracking-widest text-white shadow-xl shadow-slate-900/10 hover:bg-black transition-all"
            >
              Generar Reporte
            </button>
          </div>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-12 gap-8">
          <div class="lg:col-span-8 space-y-8">
            <section class="premium-card p-10 border border-slate-100 shadow-2xl shadow-slate-200/50 relative overflow-hidden">
              <div class="absolute -right-20 -top-20 h-64 w-64 rounded-full bg-blue-50/50"></div>
              
              <div class="relative flex flex-col md:flex-row items-start gap-8">
                <div class="flex h-24 w-24 shrink-0 items-center justify-center rounded-3xl bg-[#005792] text-white shadow-2xl shadow-blue-900/30">
                  <Building2 class="h-10 w-10" />
                </div>
                
                <div class="flex-1">
                  <div class="flex items-center gap-3 mb-2">
                    <span class="inline-flex items-center rounded-full bg-emerald-50 px-2.5 py-0.5 text-[10px] font-black uppercase tracking-widest text-emerald-600 ring-1 ring-emerald-200">
                      Certificación Activa
                    </span>
                    <span class="text-[10px] font-bold text-slate-500">ID Cliente #{{ cliente.clienteId }}</span>
                  </div>
                  <h1 class="text-4xl font-black text-slate-900 tracking-tight leading-tight">
                    {{ cliente.razonSocial }}
                  </h1>
                  <p class="mt-2 text-sm font-bold text-slate-600 uppercase tracking-[0.2em]">
                    Código Cliente: {{ cliente.codigoCliente }}
                  </p>
                </div>
              </div>
            </section>

            <div class="grid grid-cols-1 gap-8">
              <section class="premium-card p-8 border border-slate-100 shadow-xl shadow-slate-200/40">
                <h3 class="mb-8 flex items-center gap-4 text-xs font-black uppercase tracking-widest text-[#005792]">
                  <MapPin class="h-5 w-5" />
                  Ubicación de Planta
                </h3>
                
                <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
                  <div>
                    <p class="text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5 pl-1">Calle y Número</p>
                    <div class="p-4 rounded-2xl bg-slate-50 border border-slate-100 text-sm font-bold text-slate-900">
                      {{ cliente.direccion?.calle }} {{ cliente.direccion?.numero }}
                    </div>
                  </div>
                  <div>
                    <p class="text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5 pl-1">Ciudad</p>
                    <div class="p-4 rounded-2xl bg-slate-50 border border-slate-100 text-sm font-bold text-slate-900">
                      {{ cliente.direccion?.ciudad || "No especificada" }}
                    </div>
                  </div>
                </div>
              </section>
            </div>
          </div>
        </div>
      </div>

      <div v-else class="flex h-[80vh] items-center justify-center text-slate-400">
        <p class="font-bold">No se encontró el registro solicitado.</p>
      </div>
    </main>
  </div>
</template>
