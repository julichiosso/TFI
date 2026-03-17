<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { TrendingUp, PieChart, FileText, Download, CheckCircle2, XCircle, Clock, AlertCircle } from 'lucide-vue-next';
import Sidebar from '../components/Sidebar.vue';
import api from '../services/api';

const devoluciones = ref<any[]>([]);
const cargando = ref(true);

const obtenerDatos = async () => {
  try {
    const respuesta = await api.get('/api/Remito');
    devoluciones.value = respuesta.data;
  } catch (error) {
    console.error('Error al obtener datos de los reportes:', error);
  } finally {
    cargando.value = false;
  }
};

onMounted(obtenerDatos);

const estadisticas = computed(() => {
  const total = devoluciones.value.length;
  const aceptadas = devoluciones.value.filter(r => r.tipoEstado?.estado === 'Aceptada').length;
  const rechazadas = devoluciones.value.filter(r => r.tipoEstado?.estado === 'Rechazada').length;
  const pendientes = devoluciones.value.filter(r => r.tipoEstado?.estado === 'Pendiente').length;
  const enRevision = devoluciones.value.filter(r => r.tipoEstado?.estado === 'En Revisión').length;

  return {
    total,
    aceptadas,
    rechazadas,
    pendientes,
    enRevision,
    tasaAceptacion: total > 0 ? Math.round((aceptadas / total) * 100) : 0,
    tasaRechazo: total > 0 ? Math.round((rechazadas / total) * 100) : 0,
    tasaPendiente: total > 0 ? Math.round(((pendientes + enRevision) / total) * 100) : 0,
  };
});

const exportarACSV = () => {
  if (devoluciones.value.length === 0) {
    window.alert('No hay datos para exportar.');
    return;
  }
  
  const cabeceras = ['ID Remito', 'Cliente', 'Motivo', 'Items', 'Fecha', 'Estado'];
  const filas = devoluciones.value.map((r: any) => [
    r.remitoId,
    `"${r.cliente?.razonSocial || 'Desconocido'}"`,
    `"${r.motivo || ''}"`,
    r.productos?.length || 0,
    new Date(r.fecha).toLocaleDateString(),
    r.tipoEstado?.estado || 'Pendiente'
  ]);
  
  const contenidoCsv = [
    cabeceras.join(','),
    ...filas.map(f => f.join(','))
  ].join('\n');
  
  const objetoBlob = new Blob([contenidoCsv], { type: 'text/csv;charset=utf-8;' });
  const url = URL.createObjectURL(objetoBlob);
  const enlace = document.createElement('a');
  enlace.setAttribute('href', url);
  enlace.setAttribute('download', `reporte_mensual_${new Date().toISOString().split('T')[0]}.csv`);
  document.body.appendChild(enlace);
  enlace.click();
  document.body.removeChild(enlace);
};
</script>

<template>
  <div class="flex min-h-screen bg-[#fcfcfd]">
    <Sidebar />

    <main class="flex-1 p-8 max-w-7xl mx-auto overflow-y-auto no-scrollbar animate-in fade-in slide-in-from-bottom-4 duration-700">
      <div class="flex flex-col md:flex-row md:items-center justify-between gap-6 mb-12">
        <div>
          <h1 class="text-4xl font-black tracking-tighter text-slate-900 mb-2">Panel de Auditoría y Métricas</h1>
        </div>
        <button @click="exportarACSV" class="flex items-center gap-2 bg-[#005792] text-white px-8 py-4 rounded-2xl font-black text-sm uppercase tracking-widest hover:bg-[#004a7c] transition-all shadow-xl shadow-blue-900/20 active:scale-95">
          <Download class="h-5 w-5" />
          Exportar Reporte Mensual
        </button>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-12">
        <div class="bg-white p-8 rounded-3xl shadow-sm border border-slate-100 flex items-center gap-6">
          <div class="h-14 w-14 rounded-2xl bg-blue-50 flex items-center justify-center">
            <FileText class="h-7 w-7 text-[#005792]" />
          </div>
          <div>
            <span class="block text-[10px] uppercase font-black text-slate-400 tracking-widest mb-1">Total Casos</span>
            <span class="text-3xl font-black text-slate-900">{{ estadisticas.total }}</span>
          </div>
        </div>
        <div class="bg-white p-8 rounded-3xl shadow-sm border border-slate-100 flex items-center gap-6">
          <div class="h-14 w-14 rounded-2xl bg-emerald-50 flex items-center justify-center">
            <TrendingUp class="h-7 w-7 text-emerald-600" />
          </div>
          <div>
            <span class="block text-[10px] uppercase font-black text-slate-400 tracking-widest mb-1">% Aceptación</span>
            <span class="text-3xl font-black text-slate-900">{{ estadisticas.tasaAceptacion }}%</span>
          </div>
        </div>
        <div class="bg-white p-8 rounded-3xl shadow-sm border border-slate-100 flex items-center gap-6">
          <div class="h-14 w-14 rounded-2xl bg-amber-50 flex items-center justify-center">
            <Clock class="h-7 w-7 text-amber-600" />
          </div>
          <div>
            <span class="block text-[10px] uppercase font-black text-slate-400 tracking-widest mb-1">En Revisión</span>
            <span class="text-3xl font-black text-slate-900">{{ estadisticas.enRevision }}</span>
          </div>
        </div>
        <div class="bg-white p-8 rounded-3xl shadow-sm border border-slate-100 flex items-center gap-6">
          <div class="h-14 w-14 rounded-2xl bg-slate-50 flex items-center justify-center">
            <PieChart class="h-7 w-7 text-slate-600" />
          </div>
          <div>
            <span class="block text-[10px] uppercase font-black text-slate-400 tracking-widest mb-1">Días Promedio</span>
            <span class="text-3xl font-black text-slate-900">3.5</span>
          </div>
        </div>
      </div>

      <div class="max-w-4xl mx-auto mb-12">
        <div class="bg-white p-8 rounded-3xl shadow-sm border border-slate-100 animate-in fade-in slide-in-from-bottom-8 duration-1000">
          <div class="flex items-center justify-between mb-8">
            <h3 class="text-xl font-black text-slate-900 tracking-tight">Balance Operativo de Ciclos</h3>
            <PieChart class="h-6 w-6 text-slate-400" />
          </div>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-12 items-center">
            <div class="space-y-8">
              <div class="space-y-3">
                <div class="flex justify-between text-xs font-bold uppercase tracking-wider text-slate-500">
                  <span class="flex items-center gap-2"><CheckCircle2 class="h-4 w-4 text-emerald-500" /> Aceptadas</span>
                  <span class="font-black text-slate-900">{{ estadisticas.aceptadas }}</span>
                </div>
                <div class="h-4 w-full bg-slate-100 rounded-full overflow-hidden">
                  <div class="h-full bg-emerald-500 transition-all duration-1000 ease-out" :style="{ width: estadisticas.tasaAceptacion + '%' }"></div>
                </div>
              </div>
              <div class="space-y-3">
                <div class="flex justify-between text-xs font-bold uppercase tracking-wider text-slate-500">
                  <span class="flex items-center gap-2"><XCircle class="h-4 w-4 text-rose-500" /> Rechazadas</span>
                  <span class="font-black text-slate-900">{{ estadisticas.rechazadas }}</span>
                </div>
                <div class="h-4 w-full bg-slate-100 rounded-full overflow-hidden">
                  <div class="h-full bg-rose-500 transition-all duration-1000 ease-out" :style="{ width: estadisticas.tasaRechazo + '%' }"></div>
                </div>
              </div>
              <div class="space-y-3">
                <div class="flex justify-between text-xs font-bold uppercase tracking-wider text-slate-500">
                  <span class="flex items-center gap-2"><AlertCircle class="h-4 w-4 text-amber-500" /> Pendientes</span>
                  <span class="font-black text-slate-900">{{ estadisticas.pendientes + estadisticas.enRevision }}</span>
                </div>
                <div class="h-4 w-full bg-slate-100 rounded-full overflow-hidden">
                  <div class="h-full bg-amber-500 transition-all duration-1000 ease-out" :style="{ width: estadisticas.tasaPendiente + '%' }"></div>
                </div>
              </div>
            </div>

            <div class="bg-slate-50 rounded-3xl p-8 border border-slate-100">
              <div class="flex flex-col items-center text-center">
                <div class="h-24 w-24 rounded-full border-8 border-white shadow-lg bg-blue-50 flex items-center justify-center mb-4">
                  <span class="text-3xl font-black text-[#005792]">{{ estadisticas.tasaAceptacion }}%</span>
                </div>
                <h4 class="text-sm font-black text-slate-800 uppercase tracking-widest mb-2">Tasa de Resolución</h4>
                <p class="text-xs text-slate-500 font-medium leading-relaxed">
                  Porcentaje de casos aceptados sobre el total gestionado. El balance actual refleja un rendimiento operativo estable.
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>