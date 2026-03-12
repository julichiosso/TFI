<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { ShieldUser, UserPlus, Search, Edit, Trash2, ShieldCheck, UserCircle, Loader2 } from 'lucide-vue-next';
import Sidebar from '../components/Sidebar.vue';
import api from '../services/api';

const usuarios = ref<any[]>([]);
const roles = ref<any[]>([]);
const cargando = ref(true);
const consultaBusqueda = ref('');

const obtenerDatos = async () => {
  try {
    const [resUsuarios, resRoles] = await Promise.all([
      api.get('/api/Usuario'),
      api.get('/api/Rol')
    ]);
    usuarios.value = resUsuarios.data;
    roles.value = resRoles.data;
  } catch (error) {
    console.error('Error al obtener usuarios/roles:', error);
  } finally {
    cargando.value = false;
  }
};

onMounted(obtenerDatos);

const obtenerNombreRol = (rolId: number) => {
  return roles.value.find(r => r.rolId === rolId)?.nombre || 'Operador';
};

const usuariosFiltrados = computed(() => {
  return usuarios.value.filter(u =>
    u.nombre?.toLowerCase().includes(consultaBusqueda.value.toLowerCase()) ||
    u.email?.toLowerCase().includes(consultaBusqueda.value.toLowerCase())
  );
});

const estadisticas = computed(() => [
  { etiqueta: "Total Accesos", valor: usuarios.value.length },
  { etiqueta: "Administradores", valor: usuarios.value.filter(u => obtenerNombreRol(u.rolId) === 'Administrador' || obtenerNombreRol(u.rolId) === 'admin').length },
  { etiqueta: "Operadores", valor: usuarios.value.filter(u => obtenerNombreRol(u.rolId) !== 'Administrador' && obtenerNombreRol(u.rolId) !== 'admin').length },
]);

const mostrarModal = ref(false);
const esEdicion = ref(false);
const formulario = ref({ usuarioId: 0, nombre: '', email: '', passwordHash: '', rolId: 2 });
const mensajeErrorApi = ref('');
const mostrarModalEliminar = ref(false);
const usuarioAEliminar = ref<any>(null);

const abrirModal = (usuario?: any) => {
  mensajeErrorApi.value = '';
  if (usuario) {
    esEdicion.value = true;
    formulario.value = { ...usuario, passwordHash: '' };
  } else {
    esEdicion.value = false;
    formulario.value = { usuarioId: 0, nombre: '', email: '', passwordHash: '', rolId: 2 };
  }
  mostrarModal.value = true;
};

const cerrarModal = () => {
  mostrarModal.value = false;
};

const guardarUsuario = async () => {
  try {
    if (esEdicion.value) {
      await api.put(`/api/Usuario/${formulario.value.usuarioId}`, formulario.value);
    } else {
      await api.post('/api/Usuario', formulario.value);
    }
    await obtenerDatos();
    cerrarModal();
  } catch (error: any) {
    console.error('Error al guardar usuario:', error);
    let mensajeError = 'Error desconocido al guardar';
    
    if (error.response?.data) {
      if (typeof error.response.data === 'string') {
        mensajeError = error.response.data;
      } else if (error.response.data.error) {
        mensajeError = error.response.data.error;
      } else if (error.response.data.errors) {
        mensajeError = Object.values(error.response.data.errors).flat().join('\n');
      } else {
        mensajeError = JSON.stringify(error.response.data);
      }
    } else if (error.mensaje) {
      mensajeError = error.mensaje;
    }
    
    mensajeErrorApi.value = mensajeError;
  }
};

const eliminarUsuario = (usuario: any) => {
  usuarioAEliminar.value = usuario;
  mostrarModalEliminar.value = true;
};

const confirmarEliminacion = async () => {
  if (!usuarioAEliminar.value) return;
  
  try {
    await api.delete(`/api/Usuario/${usuarioAEliminar.value.usuarioId}`);
    await obtenerDatos();
    cerrarModalEliminar();
  } catch (error: any) {
    console.error('Error al eliminar usuario:', error);
    let mensaje = 'Error al eliminar el usuario.';
    if (error.response?.status === 500) {
      mensaje = 'No se puede eliminar el usuario porque tiene registros asociados (ej. remitos creados).';
    } else {
      mensaje = error.response?.data?.error || mensaje;
    }
    mensajeErrorApi.value = mensaje;
    setTimeout(() => { mensajeErrorApi.value = ''; }, 5000);
    cerrarModalEliminar();
  }
};

const cerrarModalEliminar = () => {
  mostrarModalEliminar.value = false;
  usuarioAEliminar.value = null;
};

const usuarioAutenticado = ref(JSON.parse(localStorage.getItem('weg_auth') || '{}').user || {});
const esAdmin = computed(() => {
  const rol = usuarioAutenticado.value?.rol;
  return rol === 'Administrador' || rol === 'admin';
});
</script>

<template>
  <div class="flex min-h-screen bg-[#fcfcfd]">
    <Sidebar />

    <main class="flex-1 p-8 max-w-7xl mx-auto overflow-y-auto no-scrollbar animate-in fade-in slide-in-from-bottom-4 duration-700">
      <div class="flex flex-col md:flex-row md:items-center justify-between gap-6 mb-12">
        <div>
          <h1 class="text-4xl font-black tracking-tighter text-slate-900 mb-2">Usuarios</h1>
          <p class="text-slate-500 font-medium">Gestión del personal — WEG S.A.</p>
        </div>
        <button v-if="esAdmin" @click="abrirModal()" class="flex items-center gap-2 bg-[#005792] text-white px-8 py-4 rounded-2xl font-black text-sm uppercase tracking-widest hover:bg-[#004a7c] transition-all shadow-xl shadow-blue-900/20 active:scale-95">
          <UserPlus class="h-5 w-5" />
          Nuevo Usuario
        </button>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-12">
        <div v-for="(stat, indice) in estadisticas" :key="indice" class="bg-white p-8 rounded-3xl shadow-sm border border-slate-100 flex items-center gap-6">
          <div class="h-16 w-16 rounded-2xl bg-blue-50 flex items-center justify-center">
            <ShieldUser v-if="stat.etiqueta === 'Total Accesos'" class="h-8 w-8 text-[#005792]" />
            <ShieldCheck v-else-if="stat.etiqueta === 'Administradores'" class="h-8 w-8 text-emerald-600" />
            <UserCircle v-else class="h-8 w-8 text-blue-600" />
          </div>
          <div>
            <span class="block text-[10px] uppercase font-black text-slate-400 tracking-widest mb-1">{{ stat.etiqueta }}</span>
            <span class="text-3xl font-black text-slate-900">{{ stat.valor }}</span>
          </div>
        </div>
      </div>

      <div class="bg-white p-6 rounded-3xl shadow-sm border border-slate-100 mb-8">
        <div class="relative max-w-md">
          <Search class="absolute left-4 top-1/2 -translate-y-1/2 h-5 w-5 text-slate-400" />
          <input
            v-model="consultaBusqueda"
            type="text"
            placeholder="Buscar usuario por nombre o email..."
            class="w-full pl-12 pr-6 py-4 bg-slate-50 border-none rounded-2xl text-sm font-medium focus:ring-2 focus:ring-[#005792] transition-all outline-none"
          />
        </div>
      </div>

      <div class="bg-white rounded-3xl border border-slate-100 overflow-hidden shadow-sm">
        <div v-if="cargando" class="flex flex-col items-center justify-center py-20">
          <Loader2 class="h-12 w-12 animate-spin text-[#005792]" />
          <p class="mt-4 text-slate-500 font-bold uppercase tracking-widest text-xs">Cargando personal...</p>
        </div>

        <div v-else class="overflow-x-auto">
          <table class="w-full text-left">
            <thead>
              <tr class="bg-slate-50/50">
                <th class="py-6 px-8 text-[10px] font-black uppercase tracking-widest text-slate-400">Nombre</th>
                <th class="py-6 px-4 text-[10px] font-black uppercase tracking-widest text-slate-400">Email</th>
                <th class="py-6 px-4 text-[10px] font-black uppercase tracking-widest text-slate-400">Rol</th>
                <th v-if="esAdmin" class="px-8 py-6 text-[10px] font-black uppercase tracking-widest text-slate-400 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-50">
              <tr v-for="usuario in usuariosFiltrados" :key="usuario.usuarioId" class="hover:bg-slate-50/30 transition-colors group">
                <td class="px-8 py-6">
                  <div class="flex items-center gap-4">
                    <div class="h-12 w-12 rounded-xl bg-slate-100 flex items-center justify-center">
                      <UserCircle class="h-6 w-6 text-slate-400" />
                    </div>
                    <div>
                      <span class="block text-sm font-black text-slate-900 group-hover:text-[#005792] transition-colors">{{ usuario.nombre }}</span>
                      <span class="text-xs font-bold text-slate-400 italic">ID: {{ usuario.usuarioId }}</span>
                    </div>
                  </div>
                </td>
                <td class="px-4 py-6">
                  <span class="text-sm font-medium text-slate-700">{{ usuario.email }}</span>
                </td>
                <td class="px-4 py-6">
                  <span
                    :class="[
                      'inline-flex items-center rounded-md px-2 py-1 text-xs font-bold ring-1 ring-inset',
                      (obtenerNombreRol(usuario.rolId) === 'Administrador' || obtenerNombreRol(usuario.rolId) === 'admin') ? 'bg-amber-100 text-amber-700' : 'bg-blue-100 text-blue-700'
                    ]"
                  >
                    {{ obtenerNombreRol(usuario.rolId) }}
                  </span>
                </td>
                <td v-if="esAdmin" class="px-8 py-6 text-right">
                  <div class="flex items-center justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                    <button @click="abrirModal(usuario)" class="p-2.5 text-slate-400 hover:text-[#005792] hover:bg-white rounded-xl shadow-sm transition-all border border-transparent hover:border-slate-100">
                      <Edit class="h-5 w-5" />
                    </button>
                    <button @click="eliminarUsuario(usuario)" class="p-2.5 text-slate-400 hover:text-rose-600 hover:bg-white rounded-xl shadow-sm transition-all border border-transparent hover:border-slate-100">
                      <Trash2 class="h-5 w-5" />
                    </button>
                  </div>
                </td>
              </tr>

              <tr v-if="usuariosFiltrados.length === 0 && !cargando">
                <td colspan="4" class="py-20 text-center text-slate-300">
                  <p class="text-xs font-black uppercase tracking-widest">No se encontraron usuarios</p>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div v-if="mostrarModal" class="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/50 backdrop-blur-sm p-4 animate-in fade-in duration-200">
        <div class="bg-white rounded-3xl w-full max-w-md overflow-hidden shadow-2xl relative">
          <div class="px-6 py-4 border-b border-slate-100 flex items-center justify-between bg-slate-50/50">
            <h3 class="text-lg font-black text-slate-900">{{ esEdicion ? 'Editar Usuario' : 'Nuevo Usuario' }}</h3>
            <button @click="cerrarModal" class="text-slate-400 hover:text-slate-600 transition-colors">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="h-5 w-5"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>
            </button>
          </div>
          
          <div v-if="mensajeErrorApi" class="px-6 py-3 bg-red-50 border-b border-red-100 flex items-start gap-3">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-red-500 mt-0.5 shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2"><path stroke-linecap="round" stroke-linejoin="round" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
            <p class="text-xs font-semibold text-red-700 whitespace-pre-line">{{ mensajeErrorApi }}</p>
          </div>

          <div class="p-6 space-y-4 text-sm font-medium">
            <div>
              <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5">Nombre Completo</label>
              <input v-model="formulario.nombre" type="text" class="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 focus:border-[#005792] outline-none transition-all" />
            </div>
            <div>
              <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5">Correo Electrónico</label>
              <input v-model="formulario.email" type="email" class="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 focus:border-[#005792] outline-none transition-all" />
            </div>
            <div v-if="!esEdicion">
              <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5">
                Contraseña
              </label>
              <input v-model="formulario.passwordHash" type="password" class="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 focus:border-[#005792] outline-none transition-all" />
            </div>
            <div>
              <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5">Rol de Sistema</label>
              <select v-model="formulario.rolId" class="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 focus:border-[#005792] outline-none transition-all cursor-pointer">
                <option v-for="rol in roles" :key="rol.rolId" :value="rol.rolId">{{ rol.nombre }}</option>
              </select>
            </div>
          </div>
          <div class="px-6 py-4 border-t border-slate-100 flex items-center justify-end gap-3 bg-slate-50/50">
            <button @click="cerrarModal" class="px-5 py-2.5 text-xs font-bold text-slate-500 hover:text-slate-700 transition-colors uppercase tracking-widest">Cancelar</button>
            <button @click="guardarUsuario" class="px-6 py-2.5 rounded-xl bg-[#005792] text-xs font-black text-white hover:bg-[#004a7c] transition-colors shadow-lg shadow-blue-900/20 uppercase tracking-widest">
              Guardar
            </button>
          </div>
        </div>
      </div>

      <!-- Modal de Confirmación de Eliminación Minimalista -->
      <div v-if="mostrarModalEliminar" class="fixed inset-0 z-[60] flex items-center justify-center bg-slate-900/40 backdrop-blur-[2px] p-4 animate-in fade-in duration-300">
        <div class="bg-white rounded-3xl w-full max-w-sm overflow-hidden shadow-xl border border-slate-100">
          <div class="p-8 pb-4">
            <h3 class="text-xl font-black text-slate-900 mb-2 tracking-tight">Confirmar eliminación</h3>
            <p class="text-slate-500 font-medium text-sm leading-relaxed">
              ¿Estás seguro de que deseás eliminar a <span class="text-slate-900 font-bold">{{ usuarioAEliminar?.nombre }}</span>? Esta acción no se puede deshacer.
            </p>
          </div>
          
          <div class="p-6 pt-2 flex items-center justify-end gap-3">
            <button @click="cerrarModalEliminar" class="px-5 py-3 rounded-xl text-xs font-bold text-slate-400 hover:text-slate-600 hover:bg-slate-50 transition-all uppercase tracking-widest">
              Cancelar
            </button>
            <button @click="confirmarEliminacion" class="px-6 py-3 rounded-xl bg-slate-900 text-white text-xs font-black hover:bg-rose-600 transition-all uppercase tracking-widest active:scale-95 shadow-lg shadow-slate-900/10">
              Eliminar usuario
            </button>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>