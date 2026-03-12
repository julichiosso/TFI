<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { usarAutenticacion } from "../store/auth";
import Sidebar from "../components/Sidebar.vue";
import api from "../services/api";
import {
  Plus,
  Trash2,
  Save,
  ChevronLeft,
  User,
  Package,
  AlertCircle,
  Zap,
  Upload,
  FileText,
  X,
  CheckCircle2,
  ClipboardList,
} from "lucide-vue-next";

const enrutador = useRouter();
const clientes = ref<any[]>([]);
const listaPersonal = ref<any[]>([]);
const cargando = ref(false);
const exito = ref(false);
const error = ref<string | null>(null);

const formulario = ref({
  clienteId: "",
  personalId: "",
  motivo: "",
  decisionTexto: "",
  observacionTexto: "",
  productos: [
    {
      item: "",
      modelo: "",
      cantidad: 1,
      numeroSerie: "",
      cv: null as number | null,
      rpm: null as number | null,
      voltaje: "",
    },
  ],
});

const archivoRemito = ref<File | null>(null);
const archivoNombre = ref("");
const archivoVistaPrevia = ref<string | null>(null);

onMounted(async () => {
  try {
    const [resClientes, resPersonal] = await Promise.all([
      api.get("/api/Cliente"),
      api.get("/api/Personal")
    ]);
    clientes.value = resClientes.data;
    listaPersonal.value = resPersonal.data.slice(2);
  } catch (err) {
    console.error("Error al cargar datos:", err);
    error.value = "No se pudieron cargar los datos iniciales.";
  }
});

const agregarProducto = () => {
  formulario.value.productos.push({
    item: "",
    modelo: "",
    cantidad: 1,
    numeroSerie: "",
    cv: null,
    rpm: null,
    voltaje: "",
  });
};

const eliminarProducto = (indice: number) => {
  if (formulario.value.productos.length > 1) {
    formulario.value.productos.splice(indice, 1);
  }
};

const manejarCambioArchivo = (e: any) => {
  const archivo = e.target.files[0];
  if (!archivo) return;
  archivoRemito.value = archivo;
  archivoNombre.value = archivo.name;
  if (archivo.type.startsWith("image/")) {
    const lector = new FileReader();
    lector.onload = (ev) => { archivoVistaPrevia.value = ev.target?.result as string; };
    lector.readAsDataURL(archivo);
  } else {
    archivoVistaPrevia.value = null;
  }
};

const quitarArchivo = () => {
  archivoRemito.value = null;
  archivoNombre.value = "";
  archivoVistaPrevia.value = null;
  const entrada = document.getElementById("file-upload") as HTMLInputElement;
  if (entrada) entrada.value = "";
};

const { usuario } = usarAutenticacion();

const enviarFormulario = async () => {
  if (!formulario.value.clienteId) {
    error.value = "Por favor seleccione un cliente para continuar.";
    return;
  }

  const usuarioId = usuario.value?.usuarioId;
  if (!usuarioId) {
    error.value = "Error de sesión. Por favor reingrese al sistema.";
    return;
  }

  cargando.value = true;
  error.value = null;

  try {
    let rutaArchivo = "";
    if (archivoRemito.value) {
      const datosFormulario = new FormData();
      datosFormulario.append("archivo", archivoRemito.value);
      const resCarga = await api.post("/api/Remito/subir", datosFormulario, {
        headers: { "Content-Type": "multipart/form-data" },
      });
      rutaArchivo = resCarga.data.fileName;
    }

    let idDecision = null;
    let idObservacionFormal = null;
    if (formulario.value.decisionTexto && formulario.value.personalId) {
      const personal = listaPersonal.value.find(p => p.nombre.trim().toLowerCase() === formulario.value.personalId.trim().toLowerCase());
      
      if (personal) {
        const marcaTiempo = new Date().toISOString();
        
        const resDecision = await api.post("/api/DecisionAdoptada", {
          descripcion: formulario.value.decisionTexto,
          personalId: personal.personalId,
          fecha: marcaTiempo
        });
        idDecision = resDecision.data.decisionAdoptadaId;

        const resObs = await api.post("/api/Observacion", {
          descripcion: formulario.value.decisionTexto,
          personalId: personal.personalId,
          fecha: marcaTiempo
        });
        idObservacionFormal = resObs.data.observacionId;
      } else {
        console.error(`Error de mapeo: No se encontró el área "${formulario.value.personalId}" en la base de datos.`);
        error.value = `El área "${formulario.value.personalId}" no está registrada en el sistema. Por favor contacte al administrador.`;
        cargando.value = false;
        return;
      }
    }

    const cargaUtil = {
      clienteId: parseInt(formulario.value.clienteId),
      usuarioId,
      fecha: new Date().toISOString(),
      tipoEstadoId: 1,
      motivo: formulario.value.motivo || null,
      observacionTexto: formulario.value.observacionTexto || null,
      decisionAdoptadaId: idDecision,
      observacionId: idObservacionFormal,
      archivoRemito: rutaArchivo || null,
      productos: formulario.value.productos.map((p: any) => ({
        ...p,
        cantidad: parseInt(p.cantidad.toString()),
        cv: p.cv !== null && p.cv !== undefined ? String(p.cv) : null,
        rpm: p.rpm !== null && p.rpm !== undefined ? String(p.rpm) : null,
      })),
    };

    await api.post("/api/Remito", cargaUtil);
    exito.value = true;
    setTimeout(() => enrutador.push("/returns"), 1500);
  } catch (err: any) {
    console.error("Error al crear devolución:", err);
    if (err.response?.data?.errors) {
      const erroresValidacion = err.response.data.errors;
      const claves = Object.keys(erroresValidacion);
      if (claves.length > 0) {
        const primeraClave = claves[0];
        if (primeraClave && erroresValidacion[primeraClave]) {
          const primerMensaje = erroresValidacion[primeraClave][0];
          error.value = `Error de validación: ${primerMensaje}`;
        } else {
          error.value = "Error de validación en el formulario.";
        }
      } else {
        error.value = "Error de validación en el formulario.";
      }
    } else if (err.response?.data?.message) {
      error.value = err.response.data.message;
    } else {
      error.value = "Ocurrió un error al procesar el registro.";
    }
  } finally {
    cargando.value = false;
  }
};

const clienteSeleccionado = () => clientes.value.find((c) => c.clienteId == formulario.value.clienteId);
</script>

<template>
  <div class="flex min-h-screen bg-[#f4f6f9]">
    <Sidebar />

    <main class="flex-1 overflow-y-auto">
      <div class="sticky top-0 z-10 bg-white border-b border-slate-200 px-8 py-4 flex items-center justify-between shadow-sm">
        <div class="flex items-center gap-4">
          <button
            @click="enrutador.back()"
            class="flex items-center gap-2 text-slate-400 hover:text-slate-700 transition-colors text-xs font-semibold uppercase tracking-wider"
          >
            <ChevronLeft class="h-4 w-4" />
            Volver
          </button>
          <div class="h-5 w-px bg-slate-200"></div>
          <div>
            <h1 class="text-base font-black text-slate-800 tracking-tight">Nueva Devolución</h1>
            <p class="text-[10px] text-slate-400 font-semibold uppercase tracking-widest">WEG Equipamientos Eléctricos S.A.</p>
          </div>
        </div>

        <div class="flex items-center gap-3">
          <div v-if="exito" class="flex items-center gap-2 text-emerald-600 text-xs font-bold">
            <CheckCircle2 class="h-4 w-4" />
            Registrado exitosamente
          </div>
          <button
            @click="enviarFormulario"
            :disabled="cargando || exito"
            class="flex items-center gap-2 rounded-lg bg-[#005792] px-6 py-2.5 text-sm font-bold text-white hover:bg-blue-700 transition-all disabled:opacity-50 shadow-md shadow-blue-900/20"
          >
            <Save class="h-4 w-4" />
            {{ cargando ? "Registrando..." : "Registrar Devolución" }}
          </button>
        </div>
      </div>

      <div class="max-w-6xl mx-auto px-8 py-8">
        <div
          v-if="error"
          class="mb-6 flex items-start gap-3 rounded-xl border border-red-200 bg-red-50 px-5 py-4"
        >
          <AlertCircle class="h-5 w-5 text-red-500 mt-0.5 shrink-0" />
          <div>
            <p class="text-sm font-bold text-red-700">Error al procesar</p>
            <p class="text-xs text-red-500 mt-0.5">{{ error }}</p>
          </div>
          <button @click="error = null" class="ml-auto text-red-400 hover:text-red-600">
            <X class="h-4 w-4" />
          </button>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <div class="lg:col-span-2 space-y-6">
            <section class="bg-white rounded-2xl border border-slate-200 overflow-hidden shadow-sm">
              <div class="px-6 py-4 border-b border-slate-100 flex items-center gap-3">
                <div class="h-8 w-8 rounded-lg bg-blue-50 flex items-center justify-center">
                  <User class="h-4 w-4 text-[#005792]" />
                </div>
                <div>
                  <h2 class="text-sm font-black text-slate-800">Cliente</h2>
                  <p class="text-[10px] text-slate-400 font-semibold uppercase tracking-wider">Entidad solicitante de la devolución</p>
                </div>
              </div>
              <div class="px-6 py-5">
                <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-2">
                  Razón Social <span class="text-red-400">*</span>
                </label>
                <select
                  v-model="formulario.clienteId"
                  class="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm font-semibold text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none focus:ring-3 focus:ring-blue-500/10 transition-all"
                >
                  <option value="" disabled>Seleccione una Razón Social...</option>
                  <option
                    v-for="cliente in clientes"
                    :key="cliente.clienteId"
                    :value="cliente.clienteId"
                  >
                    {{ cliente.razonSocial }}{{ cliente.codigoCliente ? ` — ${cliente.codigoCliente}` : '' }}
                  </option>
                </select>

                <div v-if="clienteSeleccionado()" class="mt-4 p-4 rounded-xl bg-blue-50 border border-blue-100 flex items-center gap-3">
                  <div class="h-10 w-10 rounded-lg bg-[#005792] flex items-center justify-center shrink-0">
                    <User class="h-5 w-5 text-white" />
                  </div>
                  <div>
                    <p class="text-sm font-black text-slate-800">{{ clienteSeleccionado()?.razonSocial }}</p>
                    <p class="text-[10px] text-slate-500 font-semibold">
                      {{ clienteSeleccionado()?.codigoCliente || 'Sin código' }} 
                      · {{ clienteSeleccionado()?.direccion?.ciudad || 'Sin localidad' }}
                    </p>
                  </div>
                </div>
              </div>
            </section>

            <section class="bg-white rounded-2xl border border-slate-200 overflow-hidden shadow-sm">
              <div class="px-6 py-4 border-b border-slate-100 flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <div class="h-8 w-8 rounded-lg bg-blue-50 flex items-center justify-center">
                    <Package class="h-4 w-4 text-[#005792]" />
                  </div>
                  <div>
                    <h2 class="text-sm font-black text-slate-800">Productos a Devolver</h2>
                    <p class="text-[10px] text-slate-400 font-semibold uppercase tracking-wider">{{ formulario.productos.length }} ítem(s) en esta devolución</p>
                  </div>
                </div>
                <button
                  @click="agregarProducto"
                  class="flex items-center gap-1.5 rounded-lg bg-slate-900 px-4 py-2 text-xs font-bold text-white hover:bg-[#005792] transition-colors"
                >
                  <Plus class="h-3.5 w-3.5" />
                  Agregar ítem
                </button>
              </div>

              <div class="divide-y divide-slate-100">
                <div
                  v-for="(prod, indice) in formulario.productos"
                  :key="indice"
                  class="px-6 py-5 relative"
                >
                  <div class="flex items-center gap-2 mb-4">
                    <span class="inline-flex h-6 w-6 items-center justify-center rounded-full bg-slate-900 text-white text-[10px] font-black">{{ indice + 1 }}</span>
                    <span class="text-xs font-bold text-slate-500 uppercase tracking-wider">Ítem {{ indice + 1 }}</span>
                    <button
                      v-if="formulario.productos.length > 1"
                      @click="eliminarProducto(indice)"
                      class="ml-auto flex items-center gap-1 text-[10px] font-bold text-slate-400 hover:text-red-500 transition-colors"
                    >
                      <Trash2 class="h-3.5 w-3.5" />
                      Eliminar
                    </button>
                  </div>

                  <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
                    <div class="md:col-span-2">
                      <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5">Producto</label>
                      <input
                        v-model="prod.item"
                        placeholder="Ej: Motor Trifásico IE3"
                        class="w-full rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 text-sm text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none transition-all"
                      />
                    </div>
                    <div>
                      <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5">Modelo <span class="text-rose-500">*</span></label>
                      <input
                        v-model="prod.modelo"
                        placeholder="Cód. Modelo"
                        class="w-full rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 text-sm text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none transition-all"
                      />
                    </div>
                    <div>
                      <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5">Cantidad <span class="text-rose-500">*</span></label>
                      <input
                        v-model="prod.cantidad"
                        type="number"
                        min="1"
                        class="w-full rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 text-sm text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none transition-all text-center font-bold"
                      />
                    </div>
                    <div>
                      <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5">N° de Serie <span class="text-rose-500">*</span></label>
                      <input
                        v-model="prod.numeroSerie"
                        placeholder="Serial"
                        class="w-full rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 text-sm text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none transition-all"
                      />
                    </div>
                    <div>
                      <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5">Potencia (CV)</label>
                      <input
                        v-model="prod.cv"
                        type="number"
                        step="0.1"
                        placeholder="—"
                        class="w-full rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 text-sm text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none transition-all"
                      />
                    </div>
                    <div>
                      <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5">Velocidad (RPM)</label>
                      <input
                        v-model="prod.rpm"
                        type="number"
                        placeholder="—"
                        class="w-full rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 text-sm text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none transition-all"
                      />
                    </div>
                    <div>
                      <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-1.5">Voltaje</label>
                      <input
                        v-model="prod.voltaje"
                        placeholder="220/380V"
                        class="w-full rounded-lg border border-slate-200 bg-slate-50 px-3 py-2.5 text-sm text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none transition-all"
                      />
                    </div>
                  </div>
                </div>
              </div>
            </section>

            <section class="bg-white rounded-2xl border border-slate-200 overflow-hidden shadow-sm">
              <div class="px-6 py-4 border-b border-slate-100 flex items-center gap-3">
                <div class="h-8 w-8 rounded-lg bg-blue-50 flex items-center justify-center">
                  <ClipboardList class="h-4 w-4 text-[#005792]" />
                </div>
                <div>
                  <h2 class="text-sm font-black text-slate-800">Motivo y Observaciones</h2>
                  <p class="text-[10px] text-slate-400 font-semibold uppercase tracking-wider">Descripción del caso</p>
                </div>
              </div>
              <div class="px-6 py-5 space-y-5">
                <div>
                  <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-2">Motivo de la Devolución</label>
                  <select
                    v-model="formulario.motivo"
                    class="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm font-semibold text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none focus:ring-3 focus:ring-blue-500/10 transition-all"
                  >
                    <option value="">Seleccione un motivo...</option>
                    <option value="Defecto de fábrica">Defecto de fábrica</option>
                    <option value="Producto dañado en transporte">Producto dañado en transporte</option>
                    <option value="Producto incorrecto">Producto incorrecto</option>
                    <option value="Falla en funcionamiento">Falla en funcionamiento</option>
                    <option value="Garantía">Garantía</option>
                    <option value="Otro">Otro</option>
                  </select>
                </div>

                <div>
                  <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-2">Observaciones Adicionales</label>
                  <textarea
                    v-model="formulario.observacionTexto"
                    rows="4"
                    placeholder="Describa el problema, condición del producto u observaciones relevantes para el área técnica..."
                    class="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none focus:ring-3 focus:ring-blue-500/10 transition-all resize-none"
                  ></textarea>
                </div>

                <div>
                  <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-2">
                    Documentación Adjunta
                    <span class="ml-2 normal-case font-semibold text-slate-400">(opcional — PDF, PNG, JPG)</span>
                  </label>

                  <div v-if="!archivoNombre">
                    <input
                      type="file"
                      accept=".pdf,.png,.jpg,.jpeg"
                      @change="manejarCambioArchivo"
                      class="hidden"
                      id="file-upload"
                    />
                    <label
                      for="file-upload"
                      class="flex flex-col items-center justify-center gap-3 cursor-pointer rounded-xl border-2 border-dashed border-slate-200 bg-slate-50 px-6 py-8 text-center hover:border-[#005792] hover:bg-blue-50/30 transition-all group"
                    >
                      <div class="h-12 w-12 rounded-xl bg-white shadow-sm border border-slate-200 flex items-center justify-center group-hover:border-[#005792] transition-colors">
                        <Upload class="h-6 w-6 text-slate-400 group-hover:text-[#005792] transition-colors" />
                      </div>
                      <div>
                        <p class="text-sm font-bold text-slate-600 group-hover:text-[#005792] transition-colors">Adjuntar remito u otro documento</p>
                        <p class="text-xs text-slate-400 mt-1">Haga clic para seleccionar o arrastre el archivo aquí</p>
                        <p class="text-[10px] text-slate-300 mt-1 font-semibold uppercase tracking-wider">PDF · PNG · JPG — Máx. 10MB</p>
                      </div>
                    </label>
                  </div>

                  <div v-else class="rounded-xl border border-slate-200 bg-white overflow-hidden">
                    <div v-if="archivoVistaPrevia" class="relative">
                      <img :src="archivoVistaPrevia" alt="Vista Previa" class="w-full max-h-48 object-cover" />
                      <div class="absolute inset-0 bg-gradient-to-t from-black/40 to-transparent"></div>
                    </div>

                    <div class="flex items-center gap-4 px-4 py-3">
                      <div class="h-10 w-10 rounded-lg flex items-center justify-center shrink-0"
                        :class="archivoVistaPrevia ? 'bg-emerald-50' : 'bg-red-50'">
                        <FileText :class="archivoVistaPrevia ? 'text-emerald-600' : 'text-red-500'" class="h-5 w-5" />
                      </div>
                      <div class="flex-1 min-w-0">
                        <p class="text-sm font-bold text-slate-800 truncate">{{ archivoNombre }}</p>
                        <p class="text-[10px] text-slate-400 font-semibold">Archivo adjunto · Listo para enviar</p>
                      </div>
                      <button
                        @click="quitarArchivo"
                        class="flex items-center gap-1.5 rounded-lg border border-red-100 bg-red-50 px-3 py-1.5 text-xs font-bold text-red-500 hover:bg-red-100 transition-colors"
                      >
                        <X class="h-3.5 w-3.5" />
                        Quitar
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </section>

            <section class="bg-white rounded-2xl border border-slate-200 overflow-hidden shadow-sm">
              <div class="px-6 py-4 border-b border-slate-100 flex items-center gap-3">
                <div class="h-8 w-8 rounded-lg bg-emerald-50 flex items-center justify-center">
                  <CheckCircle2 class="h-4 w-4 text-emerald-600" />
                </div>
                <div>
                  <h2 class="text-sm font-black text-slate-800">Decisión Adoptada</h2>
                  <p class="text-[10px] text-slate-400 font-semibold uppercase tracking-wider">Párrafo largo opcional a completar</p>
                </div>
              </div>
              <div class="px-6 py-5 space-y-5">
                <div>
                  <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-2">Personal Técnico Responsable</label>
                  <select
                    v-model="formulario.personalId"
                    class="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm font-semibold text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none transition-all"
                  >
                    <option value="">Seleccione área responsable...</option>
                    <option v-for="opt in ['Asistencia Técnica', 'Ventas', 'Facturación', 'Expedición']" :key="opt" :value="opt">{{ opt }}</option>
                  </select>
                </div>

                <div>
                  <label class="block text-[10px] font-black uppercase tracking-widest text-slate-500 mb-2">Decisión</label>
                  <textarea
                    v-model="formulario.decisionTexto"
                    rows="4"
                    placeholder="Escriba la decisión adoptada o recomendación técnica si ya fue definida..."
                    class="w-full rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm text-slate-800 focus:border-[#005792] focus:bg-white focus:outline-none transition-all resize-none"
                  ></textarea>
                </div>
              </div>
            </section>
          </div>

          <div class="space-y-5">
            <div class="bg-[#005792] rounded-2xl p-6 shadow-xl shadow-blue-900/20">
              <div class="flex items-center gap-2 mb-4">
                <Zap class="h-4 w-4 text-blue-300" />
                <h3 class="text-xs font-black uppercase tracking-widest text-blue-200">Estado Inicial</h3>
              </div>
              <div class="bg-white/10 rounded-xl px-4 py-4 mb-4">
                <p class="text-[10px] text-blue-300 uppercase tracking-widest font-bold mb-1">Estado del Registro</p>
                <p class="text-xl font-black text-white tracking-tight">PENDIENTE</p>
                <p class="mt-2 text-[11px] text-blue-200/70 leading-relaxed">
                  El registro ingresa como Pendiente y será revisado por el área técnica o administrativa.
                </p>
              </div>
              <div class="space-y-2">
                
              </div>
            </div>

            <div class="bg-white rounded-2xl border border-slate-200 p-6 shadow-sm">
              <h4 class="text-[10px] font-black uppercase tracking-widest text-slate-500 mb-4">Guía de Registro</h4>
              <div class="space-y-4">
                <div class="flex gap-3">
                  <div class="h-6 w-6 shrink-0 flex items-center justify-center rounded-full bg-slate-900 text-white text-[10px] font-black">1</div>
                  <p class="text-xs text-slate-500 leading-relaxed font-medium">Seleccione el cliente que solicita la devolución.</p>
                </div>
                <div class="flex gap-3">
                  <div class="h-6 w-6 shrink-0 flex items-center justify-center rounded-full bg-slate-900 text-white text-[10px] font-black">2</div>
                  <p class="text-xs text-slate-500 leading-relaxed font-medium">Complete el detalle de cada producto. El N° de serie es clave para la trazabilidad.</p>
                </div>
                <div class="flex gap-3">
                  <div class="h-6 w-6 shrink-0 flex items-center justify-center rounded-full bg-slate-900 text-white text-[10px] font-black">3</div>
                  <p class="text-xs text-slate-500 leading-relaxed font-medium">Indique el motivo y adjunte documentación si dispone (remito, foto, etc.).</p>
                </div>
                <div class="flex gap-3">
                  <div class="h-6 w-6 shrink-0 flex items-center justify-center rounded-full bg-[#005792] text-white text-[10px] font-black">4</div>
                  <p class="text-xs text-slate-500 leading-relaxed font-medium">Presione <span class="font-bold text-slate-700">Registrar Devolución</span> para enviar al sistema.</p>
                </div>
              </div>
            </div>

            <div v-if="formulario.clienteId" class="bg-slate-900 rounded-2xl p-6 text-white">
              <h4 class="text-[10px] font-black uppercase tracking-widest text-slate-400 mb-4">Resumen del Registro</h4>
              <div class="space-y-3">
                <div class="flex items-center justify-between">
                  <span class="text-xs text-slate-400 font-semibold">Cliente</span>
                  <span class="text-xs font-black text-white truncate max-w-[140px]">{{ clienteSeleccionado()?.razonSocial }}</span>
                </div>
                <div class="flex items-center justify-between">
                  <span class="text-xs text-slate-400 font-semibold">Ítems</span>
                  <span class="text-xs font-black text-white">{{ formulario.productos.length }}</span>
                </div>
                <div class="flex items-center justify-between">
                  <span class="text-xs text-slate-400 font-semibold">Motivo</span>
                  <span class="text-xs font-black text-white">{{ formulario.motivo || '—' }}</span>
                </div>
                <div class="flex items-center justify-between">
                  <span class="text-xs text-slate-400 font-semibold">Archivo</span>
                  <span class="text-xs font-black" :class="archivoNombre ? 'text-emerald-400' : 'text-slate-500'">
                    {{ archivoNombre ? 'Adjunto' : 'Sin adjunto' }}
                  </span>
                </div>
                <div class="pt-3 border-t border-slate-700 flex items-center justify-between">
                  <span class="text-xs text-slate-400 font-semibold">Estado</span>
                  <span class="inline-flex items-center rounded-full bg-amber-500/20 px-2.5 py-1 text-[10px] font-black text-amber-400 uppercase tracking-wider">Pendiente</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>