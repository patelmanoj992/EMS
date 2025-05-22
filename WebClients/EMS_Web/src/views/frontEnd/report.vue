
<template>
    <div class="subnavtab-wrapper">
        <div class="tab-wrapper custom-tab-wrapper">
            <ul v-if="isTabVisible" class="nav nav-tabs" role="tablist">
                <li class="nav-item" role="presentation" v-if="AuthService.CheckPermissionsPageWise(moduleName, 'MonthwiseRpt')">
                    <router-link to="/report/monthwise-rpt" custom v-slot="{ href, route, navigate }">
                        <a :class="masterRoute.meta.name == route.name || masterRoute.meta.parentNode == route.name ? ActiveClass : InActiveClass" :href="href" @click="navigate">
                            <i class="fas fa-store"></i> Monthwise                   </a>
                    </router-link>
                </li>
                <li class="nav-item" role="presentation" v-if="AuthService.CheckPermissionsPageWise(moduleName, 'ExpenseRpt')">
                    <router-link to="/report/expense-rpt" custom v-slot="{ href, route, navigate }">
                        <a :class="masterRoute.meta.name == route.name || masterRoute.meta.parentNode == route.name ? ActiveClass : InActiveClass" :href="href" @click="navigate">
                            <i class="fas fa-store"></i> Kharchi
                        </a>
                    </router-link>
                </li>
                <li class="nav-item" role="presentation" v-if="AuthService.CheckPermissionsPageWise(moduleName, 'MonthwiseSummaryRpt')">
                    <router-link to="/report/monthwise-summary" custom v-slot="{ href, route, navigate }">
                        <a :class="masterRoute.meta.name == route.name || masterRoute.meta.parentNode == route.name ? ActiveClass : InActiveClass" :href="href" @click="navigate">
                            <i class="fas fa-store"></i> Monthwise summary                   </a>
                    </router-link>
                </li>
                <!-- <li class="nav-item" role="presentation" v-if="AuthService.CheckPermissionsPageWise(moduleName, 'DivisionWiseSummaryRpt')">
                    <router-link to="/report/division-summary" custom v-slot="{ href, route, navigate }">
                        <a :class="masterRoute.meta.name == route.name || masterRoute.meta.parentNode == route.name ? ActiveClass : InActiveClass" :href="href" @click="navigate">
                            <i class="fas fa-store"></i> DivisionWise                   </a>
                    </router-link>
                </li> -->
                <li class="nav-item" role="presentation" v-if="AuthService.CheckPermissionsPageWise(moduleName, 'DepartmentWiseSummaryRpt')">
                    <router-link to="/report/department-summary" custom v-slot="{ href, route, navigate }">
                        <a :class="masterRoute.meta.name == route.name || masterRoute.meta.parentNode == route.name ? ActiveClass : InActiveClass" :href="href" @click="navigate">
                            <i class="fas fa-store"></i> Department summary                </a>
                    </router-link>
                </li>
                <!-- <li class="nav-item" role="presentation" v-if="AuthService.CheckPermissionsPageWise(moduleName, 'DesignationWiseSummaryRpt')">
                    <router-link to="/report/designation-summary" custom v-slot="{ href, route, navigate }">
                        <a :class="masterRoute.meta.name == route.name || masterRoute.meta.parentNode == route.name ? ActiveClass : InActiveClass" :href="href" @click="navigate">
                            <i class="fas fa-store"></i> DesignationWise                     </a>
                    </router-link>
                </li>      -->
                <!-- <li class="nav-item" role="presentation">
                    <router-link to="/orders/order-report" custom v-slot="{ href, route, navigate }">
                        <a :class="masterRoute.meta.name == route.name || masterRoute.meta.parentNode == route.name ? ActiveClass : InActiveClass" :href="href" @click="navigate">
                            <i class="fas fa-store"></i> Report
                        </a>
                    </router-link>
                </li>    -->
            </ul>
        </div>
        <div class="content-wrap">
            <router-view v-slot="{ Component, route }">
                <transition name="fade" mode="out-in" :key="route.path">
                    <component :is="Component" @setTabVisible="setTabVisible" />
                </transition>
            </router-view>
        </div>
    </div>
</template>
<script setup lang="ts">
    import { ref } from "vue";
    import { useRoute } from "vue-router";
    import { AuthService } from "@/composables/api/authService";
    let isTabVisible = ref(true);
    const moduleName = ref('Report')
    const setTabVisible = (visible: boolean) => {
        isTabVisible.value = AuthService.CheckPermissionsModuleWise(moduleName.value);
    };    
    const masterRoute = ref(useRoute());
    const ActiveClass = "router-link-active router-link-exact-active nav-link";
    const InActiveClass = "nav-link";
</script>
