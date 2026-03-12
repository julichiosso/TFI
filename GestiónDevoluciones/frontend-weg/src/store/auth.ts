import { reactive, computed } from 'vue'

interface Usuario {
    usuarioId?: number
    nombre: string
    rol: string
    email?: string
}

interface EstadoAutenticacion {
    usuario: Usuario | null
    token: string | null
    estaAutenticado: boolean
}

const estado = reactive<EstadoAutenticacion>({
    usuario: null,
    token: null,
    estaAutenticado: false
})

export const usarAutenticacion = () => {
    const iniciarSesion = (datosUsuario: Usuario, token: string) => {
        estado.usuario = datosUsuario
        estado.token = token
        estado.estaAutenticado = true
        localStorage.setItem('weg_auth', JSON.stringify({ user: datosUsuario, token }))
    }

    const cerrarSesion = () => {
        estado.usuario = null
        estado.token = null
        estado.estaAutenticado = false
        localStorage.removeItem('weg_auth')
    }

    const verificarAutenticacion = () => {
        const guardado = localStorage.getItem('weg_auth')
        if (guardado) {
            const datos = JSON.parse(guardado)
            estado.usuario = datos.user
            estado.token = datos.token
            estado.estaAutenticado = true
        }
    }

    return {
        usuario: computed(() => estado.usuario),
        token: computed(() => estado.token),
        estaAutenticado: computed(() => estado.estaAutenticado),
        iniciarSesion,
        cerrarSesion,
        verificarAutenticacion
    }
}
