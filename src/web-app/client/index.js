import '../node_modules/bootstrap/dist/css/bootstrap.min.css';
import Vue from 'vue';
import VueRouter from 'vue-router'
import App from './App';

Vue.use(VueRouter);

// Set global property for color theme based on user's preferences
const isDarkMode = window.matchMedia("(prefers-color-scheme: dark)").matches;
const isLightMode = window.matchMedia("(prefers-color-scheme: light)").matches;
const isNotSpecified = window.matchMedia("(prefers-color-scheme: no-preference)").matches;
const hasNoSupport = !isDarkMode && !isLightMode && !isNotSpecified;

if(isDarkMode) {
    Vue.prototype.$darkMode = true;
    document.querySelector('body').style.backgroundColor = '#212121';
}
if(isLightMode) {
    Vue.prototype.$darkMode = false;
}
if(isNotSpecified || hasNoSupport) {
    Vue.prototype.$darkMode = false;
}

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