<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import Sidebar from "../components/Sidebar.vue";
import api from "../services/api";
import { Search, Plus, ChevronRight, Filter } from "lucide-vue-next";

const consultaBusqueda = ref("");
const cargando = ref(true);
const devoluciones = ref<any[]>([]);

onMounted(async () => {
  try {
    const respuesta = await api.get("/api/Remito");
    devoluciones.value = respuesta.data.map((r: any) => ({
      id: r.remitoId ? `DEV-${r.remitoId.toString().padStart(4, "0")}` : 'DEV-????',
      remitoId: r.remitoId,
      cliente: r.cliente?.razonSocial || "Cliente Desconocido",
      productosCount: r.productos?.length || 0,
      fecha: r.fecha
        ? new Date(r.fecha).toLocaleDateString()
        : new Date().toLocaleDateString(),
      fechaCruda: r.fecha,
      estado: r.tipoEstado?.estado || "Pendiente",
      motivo: r.motivo || "",
    }));
  } catch (err) {
    console.error("Error al obtener devoluciones:", err);
  } finally {
    cargando.value = false;
  }
});

const filtroEstado = ref("");
const filtroCliente = ref("");
const fechaDesde = ref("");
const fechaHasta = ref("");

const clientesUnicos = computed(() => {
  const listaClientes = devoluciones.value.map(r => r.cliente);
  return [...new Set(listaClientes)].sort();
});

const devolucionesFiltradas = computed(() => {
  let filtradas = devoluciones.value;

  if (filtroEstado.value) {
    filtradas = filtradas.filter((dev) => dev.estado === filtroEstado.value);
  }

  if (filtroCliente.value) {
    filtradas = filtradas.filter((dev) => dev.cliente === filtroCliente.value);
  }

  if (fechaDesde.value) {
    const desde = new Date(fechaDesde.value);
    filtradas = filtradas.filter((dev) => new Date(dev.fechaCruda) >= desde);
  }

  if (fechaHasta.value) {
    const hasta = new Date(fechaHasta.value);
    hasta.setHours(23, 59, 59);
    filtradas = filtradas.filter((dev) => new Date(dev.fechaCruda) <= hasta);
  }

  if (consultaBusqueda.value) {
    const busqueda = consultaBusqueda.value.toLowerCase();
    filtradas = filtradas.filter(
      (dev: any) =>
        dev.id.toLowerCase().includes(busqueda) ||
        dev.cliente.toLowerCase().includes(busqueda) ||
        dev.motivo.toLowerCase().includes(busqueda)
    );
  }

  return filtradas;
});

const mostrarFiltros = ref(false);

const limpiarFiltros = () => {
  filtroEstado.value = "";
  filtroCliente.value = "";
  fechaDesde.value = "";
  fechaHasta.value = "";
  consultaBusqueda.value = "";
};
</script>

<template>
  <div class="flex min-h-screen bg-[#fcfcfd]">
    <Sidebar />

    <main class="flex-1 p-6 lg:p-12 overflow-y-auto no-scrollbar">
      <header class="mb-10 flex flex-col md:flex-row md:items-center justify-between gap-6">
        <div>
          <h1 class="text-4xl font-black tracking-tight text-slate-900 mb-2">
            Devoluciones
          </h1>
          <p class="text-slate-400 font-bold text-xs uppercase tracking-widest">
            Gestión de Remitos de Mercadería — WEG S.A.
          </p>
        </div>
        <router-link
          to="/returns/new"
          class="flex items-center gap-3 rounded-2xl bg-[#005792] px-8 py-4 font-black uppercase tracking-widest text-white shadow-xl shadow-blue-900/20 transition-all hover:bg-blue-700 hover:scale-105 active:scale-95"
        >
          <Plus class="h-5 w-5" />
          Nuevo Remito
        </router-link>
      </header>

      <div class="premium-card overflow-hidden">
        <div class="border-b border-slate-100 bg-white/50 p-6 space-y-4">
          <div class="flex flex-col md:flex-row items-center gap-4">
            <div class="relative group flex-1 w-full">
              <Search
                class="absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400 group-focus-within:text-[#005792] transition-colors"
              />
              <input
                v-model="consultaBusqueda"
                type="text"
                placeholder="Buscar por ID, Cliente o Motivo..."
                class="w-full rounded-2xl border border-slate-100 bg-slate-50/50 py-4 pl-12 pr-6 text-sm font-bold text-slate-900 outline-none focus:border-[#005792] focus:bg-white focus:ring-4 focus:ring-blue-500/5 transition-all"
              />
            </div>

            <select
              v-model="filtroEstado"
              class="rounded-2xl border border-slate-100 bg-white px-5 py-4 text-slate-600 hover:bg-slate-50 transition-all font-bold text-xs uppercase tracking-widest outline-none focus:ring-4 focus:ring-blue-500/5"
            >
              <option value="">Todos los Estados</option>
              <option value="Pendiente">Pendiente</option>
              <option value="En Revisión">En Revisión</option>
              <option value="Aceptada">Aceptada</option>
              <option value="Rechazada">Rechazada</option>
            </select>

            <button
              @click="mostrarFiltros = !mostrarFiltros"
              :class="['flex items-center gap-2 rounded-2xl border px-5 py-4 font-bold text-xs uppercase tracking-widest transition-all', mostrarFiltros ? 'border-[#005792] bg-blue-50 text-[#005792]' : 'border-slate-100 bg-white text-slate-500 hover:bg-slate-50']"
            >
              <Filter class="h-4 w-4" />
              Filtros
            </button>
          </div>

          <div v-if="mostrarFiltros" class="grid grid-cols-1 md:grid-cols-3 gap-4 pt-4 border-t border-slate-100 animate-in fade-in slide-in-from-top-2 duration-300">
            <div class="space-y-2">
              <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 ml-1">Cliente</label>
              <select
                v-model="filtroCliente"
                class="w-full rounded-xl border border-slate-100 bg-white px-4 py-3 text-sm font-bold text-slate-900 outline-none focus:ring-4 focus:ring-blue-500/5"
              >
                <option value="">Todos los Clientes</option>
                <option v-for="c in clientesUnicos" :key="c" :value="c">{{ c }}</option>
              </select>
            </div>
            <div class="space-y-2">
              <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 ml-1">Fecha Desde</label>
              <input
                v-model="fechaDesde"
                type="date"
                class="w-full rounded-xl border border-slate-100 bg-white px-4 py-3 text-sm font-bold text-slate-900 outline-none focus:ring-4 focus:ring-blue-500/5"
              />
            </div>
            <div class="space-y-2">
              <label class="text-[10px] font-black uppercase tracking-widest text-slate-400 ml-1">Fecha Hasta</label>
              <input
                v-model="fechaHasta"
                type="date"
                class="w-full rounded-xl border border-slate-100 bg-white px-4 py-3 text-sm font-bold text-slate-900 outline-none focus:ring-4 focus:ring-blue-500/5"
              />
            </div>
            <div class="md:col-span-3 flex justify-end">
              <button @click="limpiarFiltros" class="text-xs font-bold text-[#005792] uppercase tracking-widest hover:underline">
                Limpiar Filtros
              </button>
            </div>
          </div>

          <div class="flex items-center justify-between pt-2">
            <p class="text-[10px] font-black uppercase tracking-widest text-slate-300">
              {{ devolucionesFiltradas.length }} registros encontrados
            </p>
          </div>
        </div>

        <div class="overflow-x-auto">
          <table class="w-full text-left">
            <thead>
              <tr class="text-[10px] font-black uppercase tracking-widest text-slate-400 border-b border-slate-50">
                <th class="py-6 px-8">ID Remito</th>
                <th class="py-6 px-4">Cliente</th>
                <th class="py-6 px-4">Motivo</th>
                <th class="py-6 px-4">Items</th>
                <th class="py-6 px-4">Fecha</th>
                <th class="py-6 px-4">Estado</th>
                <th class="py-6 pr-8"></th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-50">
              <tr
                v-for="dev in devolucionesFiltradas"
                :key="dev.id"
                class="group hover:bg-slate-50/80 transition-all cursor-pointer"
                @click="$router.push(`/returns/${dev.remitoId}`)"
              >
                <td class="py-6 px-8">
                  <span
                    class="rounded-lg bg-slate-100 px-2 py-1 text-xs font-bold text-slate-600"
                    >{{ dev.id }}</span
                  >
                </td>
                <td class="py-6 px-4">
                  <p class="text-sm font-bold text-slate-900">
                    {{ dev.cliente }}
                  </p>
                </td>
                <td class="py-6 px-4">
                  <p class="text-xs font-medium text-slate-500">
                    {{ dev.motivo || "—" }}
                  </p>
                </td>
                <td class="py-6 px-4 text-center">
                  <span class="text-sm font-semibold text-slate-600">{{
                    dev.productosCount
                  }}</span>
                </td>
                <td class="py-6 px-4 text-sm font-medium text-slate-500">
                  {{ dev.fecha }}
                </td>
                <td class="py-6 px-4">
                  <span
                    :class="[
                      'inline-flex items-center rounded-full px-3 py-1 text-[10px] font-black uppercase tracking-widest shadow-sm ring-1',
                      dev.estado === 'Aceptada'
                        ? 'bg-emerald-50 text-emerald-600 ring-emerald-200'
                        : dev.estado === 'Rechazada'
                        ? 'bg-rose-50 text-rose-600 ring-rose-200'
                        : dev.estado === 'En Revisión'
                        ? 'bg-amber-50 text-amber-600 ring-amber-200'
                        : 'bg-blue-50 text-blue-600 ring-blue-200',
                    ]"
                  >
                    {{ dev.estado }}
                  </span>
                </td>
                <td class="py-6 pr-8 text-right">
                  <div
                    class="inline-flex h-10 w-10 items-center justify-center rounded-xl bg-slate-900 text-white shadow-lg shadow-slate-900/20 transition-all group-hover:scale-110 group-hover:bg-[#005792] active:scale-90"
                  >
                    <ChevronRight class="h-5 w-5" />
                  </div>
                </td>
              </tr>
              <tr v-if="devolucionesFiltradas.length === 0 && !cargando">
                <td colspan="7" class="py-20 text-center">
                  <p class="text-sm font-bold text-slate-400">No se encontraron devoluciones con los filtros seleccionados.</p>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </main>
  </div>
</template>
