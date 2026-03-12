<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import Sidebar from "../components/Sidebar.vue";
import api from "../services/api";
import {
  Package,
  Plus,
  Search,
  ChevronRight,
  CheckCircle,
  XCircle,
  Clock,
  ShieldCheck
} from "lucide-vue-next";

const enrutador = useRouter();
const consultaBusqueda = ref("");
const cargando = ref(true);

interface Estadistica {
  etiqueta: string;
  valor: string;
  descripcion: string;
  icono: any;
  color: string;
  fondoColor: string;
}

const estadisticas = ref<Estadistica[]>([
  {
    etiqueta: "Devoluciones Totales",
    valor: "0",
    descripcion: "",
    icono: Package,
    color: "text-[#005792]",
    fondoColor: "bg-blue-50"
  },
  {
    etiqueta: "En Revisión",
    valor: "0",
    descripcion: "",
    icono: Clock,
    color: "text-amber-500",
    fondoColor: "bg-amber-50"
  },
  {
    etiqueta: "Aceptadas",
    valor: "0",
    descripcion: "",
    icono: CheckCircle,
    color: "text-emerald-500",
    fondoColor: "bg-emerald-50"
  },
  {
    etiqueta: "Rechazadas",
    valor: "0",
    descripcion: "",
    icono: XCircle,
    color: "text-rose-500",
    fondoColor: "bg-rose-50"
  },
]);

const devolucionesRecientes = ref<any[]>([]);

onMounted(async () => {
  try {
    const respuesta = await api.get("/api/Remito");
    const remitos = Array.isArray(respuesta.data) ? respuesta.data : [];

    devolucionesRecientes.value = remitos.map((r: any) => ({
      id: `DEV-${r.remitoId.toString().padStart(4, "0")}`,
      remitoId: r.remitoId,
      cliente: r.cliente?.razonSocial || "Cliente Desconocido",
      productos: r.productos?.length || 0,
      fecha: r.fecha
        ? new Date(r.fecha).toLocaleDateString()
        : new Date().toLocaleDateString(),
      estado: r.tipoEstado?.estado || "En Revisión",
    }));

    if (estadisticas.value[0]) estadisticas.value[0].valor = remitos.length.toString();
    if (estadisticas.value[1]) estadisticas.value[1].valor = remitos.filter((r: any) => 
      r.tipoEstado?.estado === "En Revisión" || r.tipoEstado?.estado === "Pendiente"
    ).length.toString();
    if (estadisticas.value[2]) estadisticas.value[2].valor = remitos.filter((r: any) => r.tipoEstado?.estado === "Aceptada").length.toString();
    if (estadisticas.value[3]) estadisticas.value[3].valor = remitos.filter((r: any) => r.tipoEstado?.estado === "Rechazada").length.toString();
  } catch (error) {
    console.error("Error al obtener datos del dashboard:", error);
  } finally {
    cargando.value = false;
  }
});

const devolucionesFiltradas = computed(() => {
  if (!consultaBusqueda.value) return devolucionesRecientes.value;
  const busqueda = consultaBusqueda.value.toLowerCase();
  return devolucionesRecientes.value.filter(
    (dev: any) =>
      dev.id.toLowerCase().includes(busqueda) ||
      dev.cliente.toLowerCase().includes(busqueda)
  );
});
</script>

<template>
  <div class="flex min-h-screen bg-[#fcfcfd]">
    <Sidebar />

    <main class="flex-1 overflow-y-auto p-8 lg:p-12 no-scrollbar">
      <header class="mb-12 flex flex-col md:flex-row md:items-center justify-between gap-6">
        <div>
          <h1 class="text-5xl font-black tracking-tighter text-slate-900 mb-2">
            Dashboard
          </h1>
          <p class="text-slate-500 font-bold uppercase tracking-widest text-[10px]">
             Monitor Operativo de Garantías — WEG EQUIPAMIENTOS ELÉCTRICOS S.A.
          </p>
        </div>
        <router-link
          to="/returns/new"
          class="flex items-center gap-3 rounded-2xl bg-[#005792] px-8 py-4 font-black uppercase tracking-widest text-white shadow-2xl shadow-blue-900/20 transition-all hover:bg-blue-700 hover:scale-105 active:scale-95"
        >
          <Plus class="h-5 w-5" />
          Nueva Operación
        </router-link>
      </header>

      <div class="mb-12 grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-4">
        <div v-for="estat in estadisticas" :key="estat.etiqueta" class="premium-card p-8 group hover:shadow-2xl hover:shadow-slate-200/50 transition-all">
          <div class="mb-6 flex items-center justify-between">
            <div :class="['rounded-2xl p-4 transition-transform group-hover:scale-110', estat.fondoColor, estat.color]">
              <component :is="estat.icono" class="h-8 w-8" />
            </div>
          </div>
          <p class="text-[10px] font-black uppercase tracking-[0.2em] text-slate-400 mb-1">
            {{ estat.etiqueta }}
          </p>
          <p class="text-4xl font-black text-slate-900 mb-2">
            {{ estat.valor }}
          </p>
          <p class="text-[10px] font-bold text-slate-400 italic">{{ estat.descripcion }}</p>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-12 gap-8">
        <div class="lg:col-span-12">
          <div class="premium-card overflow-hidden h-full">
            <div class="flex items-center justify-between border-b border-slate-50 bg-white/50 p-8">
              <h2 class="text-2xl font-black tracking-tight text-slate-900">
                Resumen de Movimientos
              </h2>
              <div class="relative group">
                <Search class="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400 group-focus-within:text-[#005792]" />
                <input
                  v-model="consultaBusqueda"
                  type="text"
                  placeholder="Filtrar por ID o Cliente..."
                  class="w-48 rounded-xl border border-slate-100 bg-slate-50/50 py-2.5 pl-10 pr-4 text-xs font-bold text-slate-900 outline-none focus:border-[#005792] focus:bg-white focus:ring-4 focus:ring-blue-500/5 transition-all"
                />
              </div>
            </div>

            <div class="overflow-x-auto">
              <table class="w-full text-left">
                <thead>
                  <tr class="text-[10px] font-black uppercase tracking-widest text-slate-400">
                    <th class="py-6 px-8">N° Radicado</th>
                    <th class="py-6 px-4">Cliente</th>
                    <th class="py-6 px-4">Estado Actual</th>
                    <th class="py-6 pr-8"></th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-50">
                  <tr
                    v-for="dev in devolucionesFiltradas.slice(0, 5)"
                    :key="dev.id"
                    class="group hover:bg-slate-50/80 transition-all cursor-pointer"
                    @click="enrutador.push(`/returns/${dev.remitoId}`)"
                  >
                    <td class="py-6 px-8">
                      <span class="rounded-lg bg-slate-100 px-2 py-1 text-xs font-bold text-slate-600">{{ dev.id }}</span>
                    </td>
                    <td class="py-6 px-4">
                      <p class="text-sm font-black text-slate-900">{{ dev.cliente }}</p>
                      <p class="text-[9px] text-slate-400 uppercase font-black tracking-tighter">Ingreso: {{ dev.fecha }}</p>
                    </td>
                    <td class="py-6 px-4">
                      <span
                        :class="[
                          'inline-flex items-center rounded-full px-3 py-1 text-[9px] font-black uppercase tracking-widest shadow-sm ring-1',
                          dev.estado === 'Aceptada'
                            ? 'bg-emerald-50 text-emerald-600 ring-emerald-200'
                            : dev.estado === 'Rechazada'
                            ? 'bg-rose-50 text-rose-600 ring-rose-200'
                            : 'bg-amber-50 text-amber-600 ring-amber-200',
                        ]"
                      >
                        {{ dev.estado }}
                      </span>
                    </td>
                    <td class="py-6 pr-8 text-right">
                      <ChevronRight class="h-5 w-5 text-slate-300 group-hover:text-[#005792] transition-colors" />
                    </td>
                  </tr>
                  <tr v-if="devolucionesFiltradas.length === 0 && !cargando">
                    <td colspan="4" class="py-20 text-center text-slate-400 font-bold text-sm">
                      No hay movimientos recientes registrados.
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
            <div class="p-6 border-t border-slate-50 bg-slate-50/20 text-center">
               <router-link to="/returns" class="text-xs font-black uppercase tracking-widest text-[#005792] hover:underline">Ver Todos</router-link>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>
