import { createRouter, createWebHistory } from 'vue-router'
import { usarAutenticacion } from '../store/auth'
import DashboardView from '../views/DashboardView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/LoginView.vue')
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('../views/RegisterView.vue')
    },
    {
      path: '/',
      name: 'dashboard',
      component: DashboardView,
      meta: { requiereAutenticacion: true }
    },
    {
      path: '/returns/new',
      name: 'new-return',
      component: () => import('../views/NewReturnView.vue'),
      meta: { requiereAutenticacion: true }
    },
    {
      path: '/returns/:id',
      name: 'return-details',
      component: () => import('../views/ReturnDetailsView.vue'),
      meta: { requiereAutenticacion: true }
    },
    {
      path: '/returns',
      name: 'returns',
      component: () => import('../views/ReturnsListView.vue'),
      meta: { requiereAutenticacion: true }
    },
    {
      path: '/clients/new',
      name: 'new-client',
      component: () => import('../views/NewClientView.vue'),
      meta: { requiereAutenticacion: true }
    },
    {
      path: '/clients/:id',
      name: 'client-details',
      component: () => import('../views/ClientDetailsView.vue'),
      meta: { requiereAutenticacion: true }
    },
    {
      path: '/clients/edit/:id',
      name: 'edit-client',
      component: () => import('../views/NewClientView.vue'),
      meta: { requiereAutenticacion: true }
    },
    {
      path: '/clients',
      name: 'clients',
      component: () => import('../views/ClientsListView.vue'),
      meta: { requiereAutenticacion: true }
    },
    {
      path: '/products',
      name: 'products',
      component: () => import('../views/ProductsListView.vue'),
      meta: { requiereAutenticacion: true }
    },
    {
      path: '/products/new',
      name: 'new-product',
      component: () => import('../views/ProductFormView.vue'),
      meta: { requiereAutenticacion: true }
    },
    {
      path: '/products/edit/:id',
      name: 'edit-product',
      component: () => import('../views/ProductFormView.vue'),
      meta: { requiereAutenticacion: true }
    },
    {
      path: '/users',
      name: 'users',
      component: () => import('../views/UsersListView.vue'),
      meta: { requiereAutenticacion: true }
    },
    {
      path: '/reports',
      name: 'reports',
      component: () => import('../views/ReportsView.vue'),
      meta: { requiereAutenticacion: true }
    }
  ]
})

router.beforeEach((to, _from, next) => {
  const autenticacion = usarAutenticacion()
  autenticacion.verificarAutenticacion()

  if (to.meta.requiereAutenticacion && !autenticacion.estaAutenticado.value) {
    next('/login')
  } else if ((to.name === 'login' || to.name === 'register') && autenticacion.estaAutenticado.value) {
    next('/')
  } else {
    next()
  }
})

export default router