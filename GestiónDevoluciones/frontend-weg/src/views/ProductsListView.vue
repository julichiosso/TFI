<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { Plus, Search, Package, Edit, Trash2, Loader2 } from 'lucide-vue-next';
import Sidebar from '../components/Sidebar.vue';
import api from '../services/api';

const enrutador = useRouter();
const productos = ref<any[]>([]);
const cargando = ref(true);
const consultaBusqueda = ref('');

const obtenerProductos = async () => {
  try {
    const respuesta = await api.get('/api/Producto');
    productos.value = respuesta.data;
  } catch (error) {
    console.error('Error al obtener productos:', error);
  } finally {
    cargando.value = false;
  }
};

onMounted(obtenerProductos);

const eliminarProducto = async (id: number) => {
  if (confirm('¿Está seguro de que desea eliminar este producto?')) {
    try {
      await api.delete(`/api/Producto/${id}`);
      productos.value = productos.value.filter(p => p.productoId !== id);
    } catch (error) {
      console.error('Error al eliminar producto:', error);
      alert('Error al eliminar el producto.');
    }
  }
};

const productosFiltrados = computed(() => {
  return productos.value.filter(p =>
    p.item?.toLowerCase().includes(consultaBusqueda.value.toLowerCase()) ||
    p.modelo?.toLowerCase().includes(consultaBusqueda.value.toLowerCase())
  );
});
</script>

<template>
  <div class="flex min-h-screen bg-[#fcfcfd]">
    <Sidebar />

    <main class="flex-1 p-8 max-w-7xl mx-auto overflow-y-auto no-scrollbar animate-in fade-in slide-in-from-bottom-4 duration-700">
      <div class="flex flex-col md:flex-row md:items-center justify-between gap-6 mb-12">
        <div>
          <h1 class="text-4xl font-black tracking-tighter text-slate-900 mb-2">Fichas Técnicas</h1>
        </div>
        <button @click="enrutador.push('/products/new')" class="flex items-center gap-2 bg-[#005792] text-white px-8 py-4 rounded-2xl font-black text-sm uppercase tracking-widest hover:bg-[#004a7c] transition-all shadow-xl shadow-blue-900/20 active:scale-95">
          <Plus class="h-5 w-5" />
          Nuevo Producto
        </button>
      </div>

      <div class="bg-white p-6 rounded-3xl shadow-sm border border-slate-100 mb-8">
        <div class="relative max-w-md">
          <Search class="absolute left-4 top-1/2 -translate-y-1/2 h-5 w-5 text-slate-400" />
          <input
            v-model="consultaBusqueda"
            type="text"
            placeholder="Buscar producto por nombre o modelo..."
            class="w-full pl-12 pr-6 py-4 bg-slate-50 border-none rounded-2xl text-sm font-medium focus:ring-2 focus:ring-[#005792] transition-all outline-none"
          />
        </div>
      </div>

      <div v-if="cargando" class="flex flex-col items-center justify-center py-20">
        <Loader2 class="h-12 w-12 animate-spin text-[#005792]" />
        <p class="mt-4 text-slate-500 font-bold uppercase tracking-widest text-xs">Cargando catálogo...</p>
      </div>

      <div v-else-if="productosFiltrados.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="producto in productosFiltrados"
          :key="producto.productoId"
          class="bg-white p-6 rounded-3xl border border-slate-100 hover:shadow-2xl hover:shadow-slate-200/50 transition-all group"
        >
          <div class="flex items-start justify-between mb-6">
            <div class="h-14 w-14 rounded-2xl bg-blue-50 flex items-center justify-center group-hover:bg-[#005792] transition-colors">
              <Package class="h-7 w-7 text-[#005792] group-hover:text-white transition-colors" />
            </div>
            <div class="flex gap-2">
              <button @click="enrutador.push(`/products/edit/${producto.productoId}`)" class="p-2 text-slate-400 hover:text-[#005792] hover:bg-slate-50 rounded-xl transition-all">
                <Edit class="h-5 w-5" />
              </button>
              <button @click="eliminarProducto(producto.productoId)" class="p-2 text-slate-400 hover:text-rose-600 hover:bg-rose-50 rounded-xl transition-all">
                <Trash2 class="h-5 w-5" />
              </button>
            </div>
          </div>

          <h3 class="text-xl font-black text-slate-900 mb-2 truncate">{{ producto.item }}</h3>
          <p class="text-slate-500 font-medium text-sm mb-4">{{ producto.modelo || 'Sin modelo' }}</p>

          <div class="grid grid-cols-2 gap-4 pt-4 border-t border-slate-50">
            <div>
              <span class="block text-[10px] uppercase font-black text-slate-400 tracking-widest mb-1">Potencia</span>
              <span class="text-sm font-bold text-slate-700">{{ producto.cv || '-' }} CV</span>
            </div>
            <div>
              <span class="block text-[10px] uppercase font-black text-slate-400 tracking-widest mb-1">Voltaje</span>
              <span class="text-sm font-bold text-slate-700">{{ producto.voltaje || '-' }} V</span>
            </div>
          </div>
        </div>
      </div>

      <div v-else class="flex flex-col items-center justify-center py-20 text-slate-300">
        <Search class="h-12 w-12 opacity-20 mb-4" />
        <p class="text-xs font-black uppercase tracking-widest">No se encontraron productos</p>
      </div>
    </main>
  </div>
</template>