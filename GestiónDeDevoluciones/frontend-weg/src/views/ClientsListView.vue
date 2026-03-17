<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import Sidebar from "../components/Sidebar.vue";
import api from "../services/api";
import { Search, Plus, Filter, ChevronRight, Building2, MapPin, Edit, Trash2, AlertCircle } from "lucide-vue-next";

const consultaBusqueda = ref("");
const cargando = ref(true);
const clientes = ref<any[]>([]);
const usuarioAutenticado = ref(JSON.parse(localStorage.getItem('weg_auth') || '{}').user || {});
const mostrarModalEliminar = ref(false);
const clienteAEliminar = ref<any>(null);
const mensajeErrorApi = ref("");

const esAdmin = computed(() => {
  const rol = usuarioAutenticado.value?.rol;
  return rol === 'Administrador' || rol === 'admin';
});

const obtenerClientes = async () => {
  try {
    const respuesta = await api.get("/api/Cliente");
    clientes.value = respuesta.data;
  } catch (err) {
    console.error("Error al obtener clientes:", err);
  } finally {
    cargando.value = false;
  }
};

onMounted(obtenerClientes);

const clientesFiltrados = computed(() => {
  if (!consultaBusqueda.value) return clientes.value;
  const busqueda = consultaBusqueda.value.toLowerCase();
  return clientes.value.filter(
    (cli: any) =>
      cli.razonSocial.toLowerCase().includes(busqueda) ||
      cli.codigoCliente?.toLowerCase().includes(busqueda) ||
      cli.direccion?.ciudad?.toLowerCase().includes(busqueda)
  );
});

const eliminarCliente = (cliente: any) => {
  clienteAEliminar.value = cliente;
  mostrarModalEliminar.value = true;
};

const confirmarEliminacion = async () => {
  if (!clienteAEliminar.value) return;
  
  try {
    await api.delete(`/api/Cliente/${clienteAEliminar.value.clienteId}`);
    await obtenerClientes();
    cerrarModalEliminar();
  } catch (error: any) {
    console.error('Error al eliminar cliente:', error);
    let mensaje = 'Error al eliminar el cliente.';
    if (error.response?.status === 500) {
      mensaje = 'No se puede eliminar el cliente porque tiene registros asociados (ej. remitos).';
    } else {
      mensaje = error.response?.data?.error || mensaje;
    }
    mensajeErrorApi.value = mensaje;
    setTimeout(() => { mensajeErrorApi.value = ''; }, 5000);
  }
};

const cerrarModalEliminar = () => {
  mostrarModalEliminar.value = false;
  clienteAEliminar.value = null;
};
</script>

<template>
  <div class="flex min-h-screen bg-[#fcfcfd]">
    <Sidebar />

    <main class="flex-1 p-6 lg:p-12 overflow-y-auto no-scrollbar">
      <header class="mb-10 flex flex-col md:flex-row md:items-center justify-between gap-6">
        <div>
          <h1 class="text-4xl font-black tracking-tight text-slate-900 mb-2">
            Clientes
          </h1>
          <p class="text-slate-400 font-bold text-xs uppercase tracking-[0.2em]">
            Gestión de Entidades — WEG S.A.
          </p>
        </div>
        <router-link
          to="/clients/new"
          class="flex items-center justify-center gap-3 rounded-2xl bg-[#005792] px-8 py-4 font-black uppercase tracking-widest text-white shadow-xl shadow-blue-900/20 transition-all hover:bg-blue-700 active:scale-95"
        >
          <Plus class="h-5 w-5" />
          Registrar Cliente
        </router-link>
      </header>

      <div v-if="mensajeErrorApi" class="mb-8 p-4 bg-red-50 border-l-4 border-red-500 rounded-r-2xl animate-in fade-in slide-in-from-top-4 duration-300">
        <div class="flex items-center gap-3">
          <AlertCircle class="h-5 w-5 text-red-500" />
          <p class="text-sm font-bold text-red-700">{{ mensajeErrorApi }}</p>
        </div>
      </div>

      <div class="premium-card overflow-hidden border border-slate-100 shadow-2xl shadow-slate-200/50">
        <div class="flex flex-col md:flex-row items-center justify-between border-b border-slate-50 bg-white/50 p-8 gap-4">
          <div class="relative group w-full md:w-96">
            <Search class="absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400 group-focus-within:text-[#005792] transition-colors" />
            <input
              v-model="consultaBusqueda"
              type="text"
              placeholder="Buscar por Razón Social, Cód. o Ciudad..."
              class="w-full rounded-2xl border border-slate-100 bg-slate-50/50 py-4 pl-12 pr-6 text-sm font-bold text-slate-900 outline-none focus:border-[#005792] focus:bg-white focus:ring-4 focus:ring-blue-500/5 transition-all"
            />
          </div>

          <div class="flex items-center gap-3 w-full md:w-auto">
            <button class="flex-1 md:flex-none flex items-center justify-center gap-2 rounded-xl border border-slate-100 bg-white px-6 py-4 text-slate-400 hover:text-slate-900 hover:bg-slate-50 transition-all font-black text-[10px] uppercase tracking-widest">
              <Filter class="h-4 w-4" />
              Parámetros
            </button>
            <div class="h-10 w-px bg-slate-100 hidden md:block"></div>
            <p class="hidden md:block text-[10px] font-black uppercase tracking-widest text-slate-300">
              {{ clientesFiltrados.length }} Registros Encontrados
            </p>
          </div>
        </div>

        <div class="overflow-x-auto">
          <table class="w-full text-left">
            <thead>
              <tr class="text-[10px] font-black uppercase tracking-[0.2em] text-slate-400 border-b border-slate-50">
                <th class="py-6 px-8">Razón Social</th>
                <th class="py-6 px-4">Código Cliente</th>
                <th class="py-6 px-4">Ciudad</th>
                <th class="py-6 pr-10 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-50">
              <tr
                v-for="cliente in clientesFiltrados"
                :key="cliente.clienteId"
                class="group hover:bg-blue-50/30 transition-all cursor-pointer"
                @click="$router.push(`/clients/${cliente.clienteId}`)"
              >
                <td class="py-8 px-8">
                  <div class="flex items-center gap-5">
                    <div class="flex h-12 w-12 items-center justify-center rounded-2xl bg-white shadow-sm ring-1 ring-slate-100 group-hover:ring-[#005792]/30 transition-all">
                      <Building2 class="h-5 w-5 text-[#005792]" />
                    </div>
                    <div>
                      <p class="text-sm font-black text-slate-900 group-hover:text-[#005792] transition-colors">
                        {{ cliente.razonSocial }}
                      </p>
                      <p class="text-[10px] font-bold text-slate-400 uppercase tracking-widest">
                        ID: {{ cliente.clienteId }}
                      </p>
                    </div>
                  </div>
                </td>

                <td class="py-8 px-4">
                  <span class="text-sm font-bold text-slate-700">
                    {{ cliente.codigoCliente || '—' }}
                  </span>
                </td>

                <td class="py-8 px-4">
                  <div class="flex items-center gap-2 text-slate-500">
                    <MapPin class="h-3.5 w-3.5" />
                    <span class="text-xs font-bold">{{ cliente.direccion?.ciudad || '—' }}</span>
                  </div>
                  <p v-if="cliente.direccion?.calle" class="mt-1 text-[10px] font-medium text-slate-400 italic">
                    {{ cliente.direccion.calle }} {{ cliente.direccion.numero }}
                  </p>
                </td>

                <td class="py-8 pr-10 text-right">
                  <div class="flex items-center justify-end gap-2" @click.stop>
                    <button v-if="esAdmin" @click="$router.push(`/clients/edit/${cliente.clienteId}`)" class="p-2.5 text-slate-400 hover:text-[#005792] hover:bg-white rounded-xl shadow-sm transition-all border border-transparent hover:border-slate-100">
                      <Edit class="h-5 w-5" />
                    </button>
                    <button v-if="esAdmin" @click="eliminarCliente(cliente)" class="p-2.5 text-slate-400 hover:text-rose-600 hover:bg-white rounded-xl shadow-sm transition-all border border-transparent hover:border-slate-100">
                      <Trash2 class="h-5 w-5" />
                    </button>
                    <div class="inline-flex h-10 w-10 items-center justify-center rounded-xl bg-slate-900 text-white shadow-lg shadow-slate-900/10 transition-all group-hover:scale-110 group-hover:bg-[#005792] active:scale-95 ml-2" @click="$router.push(`/clients/${cliente.clienteId}`)">
                      <ChevronRight class="h-5 w-5" />
                    </div>
                  </div>
                </td>
              </tr>

              <tr v-if="clientesFiltrados.length === 0 && !cargando">
                <td colspan="4" class="py-20 text-center">
                  <div class="flex flex-col items-center gap-4 text-slate-300">
                    <Search class="h-12 w-12 opacity-20" />
                    <p class="text-xs font-black uppercase tracking-widest">No se detectaron coincidencias</p>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Modal de Confirmación de Eliminación -->
      <div v-if="mostrarModalEliminar" class="fixed inset-0 z-[60] flex items-center justify-center bg-slate-900/40 backdrop-blur-[2px] p-4 animate-in fade-in duration-300">
        <div class="bg-white rounded-3xl w-full max-w-sm overflow-hidden shadow-xl border border-slate-100">
          <div class="p-8 pb-4">
            <h3 class="text-xl font-black text-slate-900 mb-2 tracking-tight">Confirmar eliminación</h3>
            <p class="text-slate-500 font-medium text-sm leading-relaxed">
              ¿Estás seguro de que deseás eliminar a <span class="text-slate-900 font-bold">{{ clienteAEliminar?.razonSocial }}</span>? Esta acción no se puede deshacer.
            </p>
          </div>
          
          <div class="p-6 pt-2 flex items-center justify-end gap-3">
            <button @click="cerrarModalEliminar" class="px-5 py-3 rounded-xl text-xs font-bold text-slate-400 hover:text-slate-600 hover:bg-slate-50 transition-all uppercase tracking-widest">
              Cancelar
            </button>
            <button @click="confirmarEliminacion" class="px-6 py-3 rounded-xl bg-slate-900 text-white text-xs font-black hover:bg-rose-600 transition-all uppercase tracking-widest active:scale-95 shadow-lg shadow-slate-900/10">
              Eliminar cliente
            </button>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>