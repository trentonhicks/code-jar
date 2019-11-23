import '../node_modules/bootstrap/dist/css/bootstrap.min.css';
import Vue from 'vue';
import VueRouter from 'vue-router'
import App from './App';

Vue.use(VueRouter);

// 1. Define route components
import Home from './pages/Home';
import Login from './pages/Login';
import Admin from './pages/Admin';

// 2. Define some routes
const routes = [
    { path: '/', name: 'Home', component: Home },
    { path: '/login/', name: 'Login', component: Login },
    { path: '/admin/', name: 'Admin', component: Admin },
];

// 3. Create the router instance and pass the `routes` option
const router = new VueRouter({
    mode: 'history',
    routes
});

// 4. Create and mount the root instance.
new Vue({
    router,
    el: '#app',
    render: h => h(App),
});