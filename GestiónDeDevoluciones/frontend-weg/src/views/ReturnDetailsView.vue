<script setup lang="ts">
import { ref, onMounted, reactive } from "vue";
import { useRoute, useRouter } from "vue-router";
import Sidebar from "../components/Sidebar.vue";
import api from "../services/api";
import { usarAutenticacion } from "../store/auth";
import {
  ChevronLeft,
  Printer,
  MessageSquare,
  History,
  User,
  Calendar,
  ClipboardCheck,
  Package,
  Wrench,
  Loader2,
  FileText,
  Paperclip
} from "lucide-vue-next";

const ruta = useRoute();
const enrutador = useRouter();
const autenticacion = usarAutenticacion();
const cargando = ref(true);
const remito = ref<any>(null);
const listaPersonal = ref<any[]>([]);

const modoServicio = reactive({
  activo: false,
  guardando: false,
  observacion: "",
  decision: "",
  personalId: null as number | null,
  nuevoEstadoId: null as number | null
});

const obtenerRemito = async () => {
  try {
    const idCrudo = ruta.params.id;
    if (!idCrudo) return;
    const id = idCrudo.toString().replace("DEV-", "");
    const respuesta = await api.get(`/api/Remito/${id}`);
    remito.value = respuesta.data;
  } catch (err) {
    console.error("Error al obtener detalles del remito:", err);
  } finally {
    cargando.value = false;
  }
};

const obtenerMetadatos = async () => {
  try {
    const resPersonal = await api.get("/api/Personal");
    listaPersonal.value = resPersonal.data.slice(2);
    if (listaPersonal.value.length > 0) modoServicio.personalId = listaPersonal.value[0].personalId;
  } catch (err) {
    console.error("Error al obtener metadatos:", err);
  }
};

onMounted(() => {
  obtenerRemito();
  obtenerMetadatos();
});

const enviarAccionServicio = async () => {
  if (!modoServicio.personalId || !modoServicio.nuevoEstadoId) {
    window.alert("Debe seleccionar el personal y el nuevo estado.");
    return;
  }

  modoServicio.guardando = true;
  try {
    const id = remito.value.remitoId;

    let idObs = remito.value.observacionId;
    if (modoServicio.observacion) {
      const resObs = await api.post("/api/Observacion", {
        descripcion: modoServicio.observacion,
        personalId: modoServicio.personalId,
        fecha: new Date().toISOString()
      });
      idObs = resObs.data.observacionId;
    }

    let idDec = remito.value.decisionAdoptadaId;
    if (modoServicio.decision) {
      const resDec = await api.post("/api/DecisionAdoptada", {
        descripcion: modoServicio.decision,
        personalId: modoServicio.personalId,
        fecha: new Date().toISOString()
      });
      idDec = resDec.data.decisionAdoptadaId;
    }

    await api.put(`/api/Remito/${id}`, {
      remitoId: id,
      clienteId: remito.value.clienteId,
      usuarioId: autenticacion.usuario.value?.usuarioId || remito.value.usuarioId,
      tipoEstadoId: modoServicio.nuevoEstadoId,
      decisionAdoptadaId: idDec,
      observacionId: idObs,
      fecha: remito.value.fecha,
      motivo: remito.value.motivo,
      observacionTexto: remito.value.observacionTexto,
      archivoRemito: remito.value.archivoRemito
    });

    modoServicio.activo = false;
    modoServicio.observacion = "";
    modoServicio.decision = "";
    await obtenerRemito();
  } catch (err) {
    console.error("Error en acción de modo servicio:", err);
    window.alert("Error al procesar la acción técnica.");
  } finally {
    modoServicio.guardando = false;
  }
};

const imprimirReporte = () => {
  window.print();
};

const obtenerUrlArchivo = (nombreArchivo: string) => {
  const base = (import.meta.env.VITE_API_BASE_URL || 'http://localhost:5299').replace(/\/$/, '');
  return `${base}/uploads/${nombreArchivo}`;
};
</script>

<template>
  <div class="flex min-h-screen bg-[#fcfcfd]">
    <Sidebar />

    <main v-if="!cargando && remito" class="flex-1 p-6 lg:p-12 overflow-y-auto no-scrollbar">
      <header class="mb-10">
        <button
          @click="enrutador.back()"
          class="mb-6 flex items-center gap-2 text-slate-400 hover:text-slate-900 transition-colors font-bold text-xs uppercase tracking-widest"
        >
          <ChevronLeft class="h-4 w-4" />
          Volver al listado
        </button>

        <div class="flex flex-col md:flex-row md:items-end justify-between gap-6">
          <div class="space-y-4">
            <div class="flex items-center gap-3">
              <span class="rounded-xl bg-[#005792] px-3 py-1.5 text-xs font-black text-white shadow-xl shadow-blue-900/10 tracking-widest">
                DEV-{{ remito.remitoId.toString().padStart(4, "0") }}
              </span>
              <span
                :class="[
                  'inline-flex items-center rounded-full px-3 py-1 text-[10px] font-black uppercase tracking-widest shadow-sm ring-1',
                  remito.tipoEstado?.estado === 'Aceptada'
                    ? 'bg-emerald-50 text-emerald-600 ring-emerald-200'
                    : remito.tipoEstado?.estado === 'Rechazada'
                    ? 'bg-rose-50 text-rose-600 ring-rose-200'
                    : 'bg-amber-50 text-amber-600 ring-amber-200',
                ]"
              >
                {{ remito.tipoEstado?.estado || "Pendiente" }}
              </span>
            </div>
            <h1 class="text-4xl lg:text-5xl font-extrabold tracking-tight text-slate-900">
              Detalle del Remito
            </h1>
            <div class="flex flex-wrap items-center gap-6 text-slate-400 font-bold text-[10px] uppercase tracking-[0.2em]">
              <div class="flex items-center gap-2">
                <Calendar class="h-4 w-4 text-slate-500" />
                <span>{{ new Date(remito.fecha).toLocaleDateString() }}</span>
              </div>
              <div class="flex items-center gap-2">
                <User class="h-4 w-4 text-slate-500" />
                <span>Realizado por: {{ remito.usuario?.nombre || 'S/D' }}</span>
              </div>
            </div>
          </div>

          <div class="flex flex-col sm:flex-row gap-3 w-full sm:w-auto mt-6 md:mt-0">
            <button
              v-if="((autenticacion.usuario.value?.rol === 'Administrador' || autenticacion.usuario.value?.rol === 'admin' || autenticacion.usuario.value?.rol === 'Operador') && !modoServicio.activo)"
              @click="modoServicio.activo = true"
              class="flex items-center justify-center gap-3 rounded-2xl bg-[#005792] px-8 py-4 font-black text-xs uppercase tracking-widest text-white shadow-xl shadow-blue-900/20 hover:bg-blue-700 transition-all active:scale-95"
            >
              <Wrench class="h-5 w-5" />
              Modo Servicio
            </button>
            <button
              @click="imprimirReporte"
              class="flex items-center justify-center gap-3 rounded-2xl bg-white border border-slate-200 px-8 py-4 font-black text-xs uppercase tracking-widest text-slate-600 shadow-sm hover:bg-slate-50 transition-all active:scale-95"
            >
              <Printer class="h-5 w-5" />
              Imprimir
            </button>
          </div>
        </div>
      </header>

      <div class="grid grid-cols-1 gap-8 lg:grid-cols-12">
        <div class="lg:col-span-8 space-y-8">
          <section v-if="modoServicio.activo" class="premium-card p-10 border-2 border-[#005792]/20 bg-blue-50/10 animate-in slide-in-from-top-4 duration-500">
            <div class="flex items-center justify-between mb-10">
              <h2 class="flex items-center gap-4 text-xl font-black tracking-tight text-slate-900 uppercase">
                <div class="h-10 w-10 flex items-center justify-center rounded-xl bg-[#005792] text-white">
                  <Wrench class="h-5 w-5" />
                </div>
                Información Técnica
              </h2>
              <button @click="modoServicio.activo = false" class="text-xs font-black uppercase text-slate-500 hover:text-slate-900">Cerrar Terminal</button>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-8 mb-8">
              <div class="space-y-3">
                <label class="text-[10px] font-black uppercase tracking-widest text-slate-600 ml-1">Personal Técnico Responsable</label>
                <select v-model="modoServicio.personalId" class="w-full rounded-2xl border border-slate-200 bg-white p-4 text-sm font-bold text-slate-900 outline-none focus:ring-4 focus:ring-blue-500/5 transition-all">
                  <option v-for="p in listaPersonal" :key="p.personalId" :value="p.personalId">{{ p.nombre }}</option>
                </select>
              </div>
              <div class="space-y-3">
                <label class="text-[10px] font-black uppercase tracking-widest text-slate-600 ml-1">Estado de la Resolución</label>
                <div class="grid grid-cols-2 gap-2">
                  <button @click="modoServicio.nuevoEstadoId = 3" :class="['px-4 py-3 rounded-xl text-[10px] font-black uppercase tracking-widest transition-all', modoServicio.nuevoEstadoId === 3 ? 'bg-emerald-600 text-white shadow-lg' : 'bg-white border border-slate-200 text-slate-900 hover:bg-slate-50']">ACEPTAR</button>
                  <button @click="modoServicio.nuevoEstadoId = 4" :class="['px-4 py-3 rounded-xl text-[10px] font-black uppercase tracking-widest transition-all', modoServicio.nuevoEstadoId === 4 ? 'bg-rose-600 text-white shadow-lg' : 'bg-white border border-slate-200 text-slate-900 hover:bg-slate-50']">RECHAZAR</button>
                  <button @click="modoServicio.nuevoEstadoId = 2" :class="['px-4 py-3 rounded-xl text-[10px] font-black uppercase tracking-widest transition-all', modoServicio.nuevoEstadoId === 2 ? 'bg-amber-500 text-white shadow-lg' : 'bg-white border border-slate-200 text-slate-900 hover:bg-slate-50']">EN REVISIÓN</button>
                </div>
              </div>
            </div>

            <div class="space-y-8">
              <div class="space-y-3">
                <label class="text-[10px] font-black uppercase tracking-widest text-slate-600 ml-1">Informe de Hallazgos</label>
                <textarea v-model="modoServicio.observacion" rows="3" placeholder="Describa el fallo técnico o estado de los componentes detectados..." class="w-full rounded-2xl border border-slate-200 bg-white p-6 text-sm font-bold text-slate-900 outline-none focus:ring-4 focus:ring-blue-500/5 transition-all resize-none"></textarea>
              </div>
              <div class="space-y-3">
                <label class="text-[10px] font-black uppercase tracking-widest text-slate-600 ml-1">Descripción</label>
                <textarea v-model="modoServicio.decision" rows="2" placeholder="Recomendación técnica..." class="w-full rounded-2xl border border-slate-200 bg-white p-6 text-sm font-bold text-slate-900 outline-none focus:ring-4 focus:ring-blue-500/5 transition-all resize-none"></textarea>
              </div>
            </div>

            <div class="mt-10 pt-8 border-t border-slate-100 flex justify-end">
              <button
                @click="enviarAccionServicio"
                :disabled="modoServicio.guardando"
                class="flex items-center gap-3 rounded-2xl bg-[#005792] px-10 py-4 font-black text-xs uppercase tracking-widest text-white shadow-xl shadow-blue-900/20 hover:bg-blue-700 transition-all disabled:opacity-50"
              >
                <Loader2 v-if="modoServicio.guardando" class="h-5 w-5 animate-spin" />
                <span>{{ modoServicio.guardando ? 'Procesando...' : 'Consolidar Acta de Servicio' }}</span>
              </button>
            </div>
          </section>

          <div class="premium-card p-8 grid grid-cols-1 md:grid-cols-2 gap-8 shadow-xl shadow-slate-200/50">
            <div class="space-y-5">
              <h3 class="text-[10px] font-black uppercase tracking-[0.2em] text-[#005792]">Cliente</h3>
              <div class="space-y-1">
                <p class="text-xl font-black text-slate-900">{{ remito.cliente?.razonSocial }}</p>
                <p class="text-xs font-bold text-slate-400 italic">Identificador: {{ remito.cliente?.codigoCliente }}</p>
              </div>
              <p class="text-xs font-bold text-slate-500 leading-relaxed bg-slate-50 p-4 rounded-2xl border border-slate-100">
                {{ remito.cliente?.direccion?.calle }} {{ remito.cliente?.direccion?.numero }}<br>
                {{ remito.cliente?.direccion?.ciudad }}
              </p>
            </div>
            <div class="space-y-5">
              <h3 class="text-[10px] font-black uppercase tracking-[0.2em] text-[#005792]">Control de Existencias</h3>
              <div class="grid grid-cols-2 gap-4">
                <div class="bg-white border border-slate-100 shadow-sm rounded-2xl p-5 text-center">
                  <p class="text-3xl font-black text-[#005792]">{{ remito.productos?.length || 0 }}</p>
                  <p class="mt-1 text-[8px] font-black uppercase text-slate-400 tracking-tighter">Devoluciones</p>
                </div>
                <div class="bg-white border border-slate-100 shadow-sm rounded-2xl p-5 text-center">
                  <p class="text-3xl font-black text-[#005792]">{{ remito.productos?.reduce((acc: number, p: any) => acc + (p.cantidad || 0), 0) || 0 }}</p>
                  <p class="mt-1 text-[8px] font-black uppercase text-slate-400 tracking-tighter">Unidades</p>
                </div>
              </div>
            </div>
          </div>

          <div class="premium-card p-8 shadow-xl shadow-slate-200/50">
            <h2 class="flex items-center gap-3 text-xl font-black tracking-tight text-slate-900 mb-8">
              <Package class="h-6 w-6 text-[#005792]" />
              Productos
            </h2>
            <div class="overflow-x-auto">
              <table class="w-full text-left min-w-[600px]">
                <thead>
                  <tr class="text-[10px] font-black uppercase tracking-widest text-slate-400 border-b border-slate-100">
                    <th class="pb-6 px-4">Producto</th>
                    <th class="pb-6 px-4">Configuración</th>
                    <th class="pb-6 px-4 text-center">Unid.</th>
                    <th class="pb-6 px-4">Serial / ID</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-50">
                  <tr v-for="(prod, indice) in remito.productos" :key="indice" class="group transition-all hover:bg-slate-50/50">
                    <td class="py-6 px-4">
                      <p class="text-sm font-black text-slate-900">{{ prod.item }}</p>
                      <p class="text-[10px] font-bold text-[#005792] uppercase tracking-widest">{{ prod.modelo }}</p>
                    </td>
                    <td class="py-6 px-4">
                      <div class="flex gap-1.5 flex-wrap">
                        <span v-if="prod.cv" class="rounded-lg bg-slate-900 px-2 py-1 text-[8px] font-black text-white uppercase tracking-tighter">{{ prod.cv }} CV</span>
                        <span v-if="prod.rpm" class="rounded-lg bg-slate-50 px-2 py-1 text-[8px] font-black text-slate-600 uppercase tracking-tighter border border-slate-100">{{ prod.rpm }} RPM</span>
                        <span v-if="prod.voltaje" class="rounded-lg bg-slate-50 px-2 py-1 text-[8px] font-black text-slate-600 uppercase tracking-tighter border border-slate-100">{{ prod.voltaje }}V</span>
                      </div>
                    </td>
                    <td class="py-6 px-4 text-center">
                      <span class="inline-flex h-8 w-8 items-center justify-center rounded-xl bg-slate-50 border border-slate-100 text-xs font-black text-slate-900">{{ prod.cantidad }}</span>
                    </td>
                    <td class="py-6 px-4">
                      <span class="font-mono text-xs font-bold text-slate-500">{{ prod.numeroSerie || 'S/N' }}</span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
            <div class="premium-card p-8 shadow-xl shadow-slate-200/50">
              <h3 class="text-[10px] font-black uppercase tracking-[0.2em] text-[#005792] mb-4">
                <FileText class="h-4 w-4 inline mr-2" />Motivo
              </h3>
              <p class="text-sm font-bold text-slate-700">{{ remito.motivo || 'No especificado' }}</p>
            </div>
            <div class="premium-card p-8 shadow-xl shadow-slate-200/50">
              <h3 class="text-[10px] font-black uppercase tracking-[0.2em] text-[#005792] mb-4">Observaciones del Operador</h3>
              <p class="text-sm font-bold text-slate-700 italic">{{ remito.observacionTexto || 'Sin observaciones' }}</p>
            </div>
            <div class="premium-card p-8 shadow-xl shadow-slate-200/50">
              <h3 class="text-[10px] font-black uppercase tracking-[0.2em] text-[#005792] mb-4">
                <Paperclip class="h-4 w-4 inline mr-2" />Archivo Adjunto
              </h3>
              <a
                v-if="remito.archivoRemito"
                :href="obtenerUrlArchivo(remito.archivoRemito)"
                target="_blank"
                class="inline-flex items-center gap-2 text-sm font-bold text-[#005792] hover:underline"
              >
                Ver Archivo
              </a>
              <p v-else class="text-sm font-bold text-slate-400">Sin archivo adjunto</p>
            </div>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
            <div class="premium-card p-10 border-l-4 border-l-purple-500 shadow-xl shadow-slate-200/40">
              <h2 class="mb-8 flex items-center gap-3 text-[10px] font-black uppercase tracking-[0.3em] text-slate-400">
                <MessageSquare class="h-4 w-4 text-purple-500" />
                Observaciones Técnicas
              </h2>
              <div v-if="remito.observacion" class="space-y-6">
                <p class="text-sm font-bold leading-relaxed text-slate-700 italic border-l-2 border-purple-100 pl-4 py-2">
                  "{{ remito.observacion.descripcion }}"
                </p>
                <p class="text-[10px] font-black text-slate-400">{{ remito.observacion.personal?.nombre || 'Operador - Técnico' }}</p>
              </div>
              <div v-else class="py-8 text-center bg-slate-50/50 rounded-3xl border border-dashed border-slate-200">
                <p class="text-[10px] font-black uppercase tracking-widest text-slate-300">Pendiente de Informe Técnico</p>
              </div>
            </div>

            <div class="premium-card p-10 border-l-4 border-l-[#005792] shadow-xl shadow-slate-200/40">
              <h2 class="mb-8 flex items-center gap-3 text-[10px] font-black uppercase tracking-[0.3em] text-slate-400">
                <ClipboardCheck class="h-4 w-4 text-[#005792]" />
                Decisión Adoptada
              </h2>
              <div v-if="remito.decision" class="space-y-6">
                <div class="!bg-[#005792] rounded-2xl p-6 text-white shadow-lg shadow-blue-900/20">
                  <p class="text-[8px] font-black uppercase tracking-[0.2em] opacity-60 mb-2">Descripción</p>
                  <p class="text-xs font-bold leading-relaxed">{{ remito.decision.descripcion }}</p>
                </div>
                <div class="flex items-center gap-3 text-[#005792]">
                  <Calendar class="h-4 w-4" />
                  <span class="text-[10px] font-black uppercase tracking-widest">{{ new Date(remito.decision.fecha).toLocaleDateString() }}</span>
                </div>
              </div>
              <div v-else class="py-8 text-center bg-slate-50/50 rounded-3xl border border-dashed border-slate-200">
                <p class="text-[10px] font-black uppercase tracking-widest text-slate-300">A la espera de resolución</p>
              </div>
            </div>
          </div>
        </div>

        <div class="lg:col-span-4 space-y-8">
          <section class="premium-card p-8 border border-slate-100 shadow-xl shadow-slate-200/30">
            <h3 class="mb-10 flex items-center gap-3 text-[10px] font-black uppercase tracking-[0.3em] text-slate-400">
              <History class="h-4 w-4 text-[#005792]" />
              Cronología de Garantía
            </h3>

            <div class="relative pl-6 space-y-12 before:absolute before:left-0 before:top-1.5 before:h-full before:w-[1px] before:bg-slate-100">
              <div class="relative">
                <div class="absolute -left-[27.5px] top-0 h-4 w-4 rounded-full bg-slate-900 border-4 border-white shadow-md"></div>
                <div>
                  <p class="text-[11px] font-black text-slate-900 uppercase tracking-widest">Apertura de Expediente</p>
                  <p class="text-[10px] font-bold text-slate-400 mt-1 italic">{{ new Date(remito.fecha).toLocaleDateString() }}</p>
                  <p class="mt-2 text-[9px] font-bold text-slate-500 bg-slate-50 p-2 rounded-lg border border-slate-100">Iniciado por: {{ remito.usuario?.nombre || 'Terminal 01' }}</p>
                </div>
              </div>

              <div v-if="remito.tipoEstadoId !== 1" class="relative">
                <div :class="['absolute -left-[27.5px] top-0 h-4 w-4 rounded-full border-4 border-white shadow-md', remito.tipoEstadoId === 3 ? 'bg-emerald-500' : remito.tipoEstadoId === 4 ? 'bg-rose-500' : 'bg-amber-500']"></div>
                <div>
                  <p :class="['text-[11px] font-black uppercase tracking-widest', remito.tipoEstadoId === 3 ? 'text-emerald-600' : remito.tipoEstadoId === 4 ? 'text-rose-600' : 'text-amber-600']">
                    Estado: {{ remito.tipoEstado?.estado }}
                  </p>
                </div>
              </div>

              <div class="relative">
                <div class="absolute -left-[27.5px] top-0 h-4 w-4 rounded-full bg-slate-100 border-4 border-white"></div>
                <p class="text-[11px] font-black text-slate-300 uppercase tracking-widest">Cierre de Proceso</p>
              </div>
            </div>
          </section>
        </div>
      </div>
    </main>

    <main v-else class="flex-1 p-12 flex items-center justify-center">
      <div class="flex flex-col items-center gap-6">
        <Loader2 class="h-12 w-12 animate-spin text-[#005792]" />
        <p class="text-[10px] font-black uppercase tracking-[0.3em] text-slate-400 animate-pulse">Sincronizando con Servidor...</p>
      </div>
    </main>
  </div>
</template>

<style scoped>
@media print {
  aside, button { display: none !important; }
  main { padding: 0 !important; }
  .premium-card { border: 1px solid #eee !important; box-shadow: none !important; margin-bottom: 2rem !important; }
}
</style>