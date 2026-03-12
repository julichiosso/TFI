import axios from 'axios'

const URL_BASE_API = 'http://localhost:5299'

const api = axios.create({
  baseURL: URL_BASE_API,
  headers: {
    'Content-Type': 'application/json'
  }
})

api.interceptors.request.use((config) => {
  const datosAutenticacion = localStorage.getItem('weg_auth')
  if (datosAutenticacion) {
    const { token } = JSON.parse(datosAutenticacion)
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
  }
  return config
})

export default api
