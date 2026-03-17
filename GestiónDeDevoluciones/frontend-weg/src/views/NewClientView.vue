<script setup lang="ts">
import { ref, reactive, onMounted, computed } from "vue";
import { useRouter, useRoute } from "vue-router";
import Sidebar from "../components/Sidebar.vue";
import api from "../services/api";
import {
  MapPin,
  Building2,
  ChevronLeft,
  Save,
  CheckCircle2,
  AlertCircle
} from "lucide-vue-next";

const enrutador = useRouter();
const ruta = useRoute();
const cargando = ref(false);
const error = ref<string | null>(null);
const exito = ref(false);

const esEdicion = computed(() => !!ruta.params.id);
const idCliente = computed(() => ruta.params.id);
const usuarioAutenticado = ref(JSON.parse(localStorage.getItem('weg_auth') || '{}').user || {});
const esAdmin = computed(() => {
  const rol = usuarioAutenticado.value?.rol;
  return rol === 'Administrador' || rol === 'admin';
});

const formulario = reactive({
  razonSocial: "",
  codigoCliente: "",
  calle: "",
  numero: "",
  ciudad: "",
});

onMounted(async () => {
  if (esEdicion.value) {
    cargando.value = true;
    try {
      const respuesta = await api.get(`/api/Cliente/${idCliente.value}`);
      const datos = respuesta.data;
      formulario.razonSocial = datos.razonSocial;
      formulario.codigoCliente = datos.codigoCliente;
      if (datos.direccion) {
        formulario.calle = datos.direccion.calle;
        formulario.numero = datos.direccion.numero;
        formulario.ciudad = datos.direccion.ciudad;
      }
    } catch (err) {
      console.error("Error al cargar cliente:", err);
      error.value = "No se pudo cargar la información del cliente.";
    } finally {
      cargando.value = false;
    }
  }
});

const enviarFormulario = async () => {
  if (!formulario.razonSocial) {
    error.value = "Por favor, complete los campos obligatorios (*)";
    return;
  }

  cargando.value = true;
  error.value = null;

  try {
    const cargaUtil = {
      razonSocial: formulario.razonSocial,
      codigoCliente: formulario.codigoCliente,
      direccion: {
        calle: formulario.calle || "S/D",
        numero: formulario.numero || "0",
        ciudad: formulario.ciudad,
      },
    };

    if (esEdicion.value) {
      await api.put(`/api/Cliente/${idCliente.value}`, cargaUtil);
    } else {
      await api.post("/api/Cliente", cargaUtil);
    }

    exito.value = true;
    setTimeout(() => {
      enrutador.push(esEdicion.value ? `/clients/${idCliente.value}` : "/clients");
    }, 2000);
  } catch (err: any) {
    console.error("Error al guardar cliente:", err);
    error.value = err.response?.data?.message || "Error al procesar la solicitud en el servidor.";
  } finally {
    cargando.value = false;
  }
};
</script>

<template>
  <div class="flex min-h-screen bg-[#fcfcfd]">
    <Sidebar />

    <main class="flex-1 p-6 lg:p-12 overflow-y-auto no-scrollbar">
      <header class="mb-10">
        <button
          @click="enrutador.back()"
          class="mb-6 flex items-center gap-2 text-slate-400 hover:text-slate-900 transition-colors font-bold text-xs uppercase tracking-widest"
        >
          <ChevronLeft class="h-4 w-4" />
          Volver al directorio
        </button>

        <div class="flex flex-col md:flex-row md:items-center justify-between gap-6">
          <div>
            <h1 class="text-4xl font-extrabold tracking-tight text-slate-900 mb-2">
              {{ esEdicion ? 'Editar Cliente' : 'Nuevo Cliente' }}
            </h1>
            <p class="text-slate-400 font-bold text-xs uppercase tracking-widest">
              {{ esEdicion ? 'Actualización de Datos de Entidad' : 'Registro de Entidad Autorizada para Garantías WEG' }}
            </p>
          </div>

          <button
            @click="enviarFormulario"
            :disabled="cargando || exito || (esEdicion && !esAdmin)"
            class="flex items-center justify-center gap-3 rounded-2xl bg-[#005792] px-8 py-4 font-black uppercase tracking-widest text-white shadow-xl shadow-blue-900/20 transition-all hover:bg-blue-700 active:scale-95 disabled:opacity-50"
          >
            <span v-if="cargando">Procesando...</span>
            <template v-else-if="exito">
              <CheckCircle2 class="h-5 w-5" />
              ¡{{ esEdicion ? 'Actualizado' : 'Registrado' }}!
            </template>
            <template v-else>
              <Save class="h-5 w-5" />
              {{ esEdicion ? 'Guardar Cambios' : 'Guardar Cliente' }}
            </template>
          </button>
        </div>
      </header>

      <div class="grid grid-cols-1 lg:grid-cols-12 gap-8">
        <div class="lg:col-span-8 space-y-8 h-fit">
          <section class="premium-card p-10 border border-slate-100 shadow-2xl shadow-slate-200/50">
            <h2 class="mb-10 flex items-center gap-4 text-xl font-bold tracking-tight text-slate-900">
              <Building2 class="h-6 w-6 text-[#005792]" />
              Razón Social
            </h2>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
              <div class="space-y-3">
                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 ml-2">Razón Social *</label>
                <input
                  v-model="formulario.razonSocial"
                  type="text"
                  placeholder="Ej: Distribuidora Norte S.A."
                  class="w-full rounded-2xl border border-slate-100 bg-slate-50/50 px-6 py-4 text-sm font-bold text-slate-900 outline-none focus:border-[#005792] focus:bg-white focus:ring-4 focus:ring-blue-900/5 transition-all"
                />
              </div>

              <div class="space-y-3">
                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 ml-2">Código Cliente</label>
                <input
                  v-model="formulario.codigoCliente"
                  type="text"
                  placeholder="Ej: CLI-2026-X"
                  class="w-full rounded-2xl border border-slate-100 bg-slate-50/50 px-6 py-4 text-sm font-bold text-slate-900 outline-none focus:border-[#005792] focus:bg-white focus:ring-4 focus:ring-blue-900/5 transition-all"
                />
              </div>
            </div>
          </section>

          <section class="premium-card p-10 border border-slate-100 shadow-2xl shadow-slate-200/50">
            <h2 class="mb-10 flex items-center gap-4 text-xl font-bold tracking-tight text-slate-900">
              <MapPin class="h-6 w-6 text-[#005792]" />
              Dirección
            </h2>

            <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
              <div class="md:col-span-2 space-y-3">
                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 ml-2">Calle</label>
                <input
                  v-model="formulario.calle"
                  type="text"
                  placeholder="Nombre de la vía"
                  class="w-full rounded-2xl border border-slate-100 bg-slate-50/50 px-6 py-4 text-sm font-bold text-slate-900 outline-none focus:border-[#005792] focus:bg-white focus:ring-4 focus:ring-blue-900/5 transition-all"
                />
              </div>

              <div class="space-y-3">
                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 ml-2">Número</label>
                <input
                  v-model="formulario.numero"
                  type="text"
                  placeholder="Altura"
                  class="w-full rounded-2xl border border-slate-100 bg-slate-50/50 px-6 py-4 text-sm font-bold text-slate-900 outline-none focus:border-[#005792] focus:bg-white focus:ring-4 focus:ring-blue-900/5 transition-all"
                />
              </div>

              <div class="md:col-span-3 space-y-3">
                <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 ml-2">Ciudad / Provincia</label>
                <input
                  v-model="formulario.ciudad"
                  type="text"
                  placeholder="Ciudad de radicación"
                  class="w-full rounded-2xl border border-slate-100 bg-slate-50/50 px-6 py-4 text-sm font-bold text-slate-900 outline-none focus:border-[#005792] focus:bg-white focus:ring-4 focus:ring-blue-900/5 transition-all"
                />
              </div>
            </div>
          </section>
        </div>

        <div class="lg:col-span-4 space-y-8">
          <div v-if="error" class="premium-card p-8 border-l-4 border-l-rose-500 bg-rose-50 animate-in shake duration-500">
            <div class="flex items-start gap-4">
              <AlertCircle class="h-6 w-6 text-rose-600 mt-1 shrink-0" />
              <div>
                <h4 class="text-xs font-black uppercase tracking-widest text-rose-900">Requerimiento</h4>
                <p class="mt-2 text-xs font-bold text-rose-600 leading-relaxed">{{ error }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>