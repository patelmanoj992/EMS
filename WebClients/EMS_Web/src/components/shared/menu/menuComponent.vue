<template>
    <div class="sidenav-layout" :class="{ 'layout-sidebar-active': sidebarActive }">
        <div class="menu-div">
            <div class="user-profile-dropdown">
                <Button type="button" @click="toggle" aria-haspopup="true" aria-controls="overlay_menu">
                    <span class="img-wrapper"><i class="pi pi-user"></i></span>
                    <span>John Doe</span>
                    <i class="pi pi-chevron-down"></i>
                </Button>
                <Menu id="overlay_menu" class="fixedTopMenu" ref="menu" :model="items" :popup="true" />
            </div>

            <router-link v-if="IsShowBackButton" class="Back-btn" :to="IsBackPath">
                <i class="pi pi-chevron-left"></i>
            </router-link>

            <ul class="main-menu">
                <li v-for="items in Menulist.filter((x) => { return x.IsFrontMenu == Props.IsAdminMenu && x.IsShow === true;})" :key="items.index" v-bind:class="{ active: items.MainPageCode ===
              String(routes?.meta?.MainPageCode ?? routes?.name)}">
                    <router-link :to="items.to || '#'" custom v-slot="{ href, navigate, route}">
                        <a :href="href" @click="navigate" :class="{ 'active-link': routes.meta.MainPage == route.meta.MainPage , 'exact-active-link': routes.meta.MainPage == route.meta.MainPage }">
                            <span class="p-menuitem-icon pi" v-bind:class="items.icon"></span>
                            <span>{{ items.label }}</span>
                        </a>
                    </router-link>
                </li>
            </ul>

        </div>
    </div>
    
    <div class="sidenav-overlay" @click="$emit('sideNavClose', $event)"></div>

</template>

<script setup lang="ts">
    import { defineProps, defineEmits, onMounted, inject, ref, watch } from "vue";
    import menuModule from "@/composables/modules/layout/menuModule";
    import { useRoute } from "vue-router";    
    import { AuthService } from "@/composables/api/authService";
    //-------------------------------------Propertise--------------------------------------------------//
    const emits = defineEmits(["sideNavClose"]);
    const {
        updatepopupRefs,
        isActive,
        openedItems,
        collspasedItem,
        //AdminMenuList,
        SubMenucollspasedItem,
        //useLang,
        getCurrentMenuSelection,
    } = new menuModule();

    const Props = defineProps({
        sidebarActive: {
            type: Boolean,
        },
        IsAdminMenu: { type: Boolean, default: false },
        IsShowBackButton: { type: Boolean, default: false },
        IsBackPath: { type: String, default: "/home" },
    });
    const menu = ref();
    const items = ref([
        {
            icon: "pi pi-user",
            label: "User Profile",
            //to: "/User/Profile",
            // command: ()=> {
            //     IsFrontMenu.value=true
            // }
        },
        {
            icon: "pi pi-sign-out",
            label: "Logout",
            to: "/",
        },
    ]);
    const toggle = (event) => {
        menu.value.toggle(event);
    };
    
    let prm = ['User','Roles&Permissions','Setting','AuditTrail','Master','Sales','Technical']
    const Menulist = ref([
        {
            label: "Dashboard",
            to: "/Dashboard",
            icon: "pi-th-large",
            IsFrontMenu: true,
            IsShow: true            
        },
        {
            label: "Master",
            to: AuthService.getMultipleModulePagePath(['Masters'],['Employee'],'Employee','master'),//"/Master",
            icon: "pi-th-large",
            IsFrontMenu: true,
            IsShow: AuthService.CheckPermissionsModuleWise('Masters')
        },
        {
            label: "Activation",
            to: AuthService.getMultipleModulePagePath(['Attendance'],['Activation'],'Activation', 'activation'),
            icon: "pi-th-large",
            IsFrontMenu: true,
            IsShow: AuthService.CheckPermissionsModuleWise('Attendance')
        } ,{
            label: "Kharchi",
            to: AuthService.getMultipleModulePagePath(['Expense'],['Expense'],'Expense', 'expense'),
            icon: "pi-th-large",
            IsFrontMenu: true,
            IsShow: AuthService.CheckPermissionsModuleWise('Attendance')
        } ,{
            label: "Attendance",
            to: AuthService.getMultipleModulePagePath(['Attendance'],['Attendance'],'Attendance', 'attendance'),
            icon: "pi-th-large",
            IsFrontMenu: true,
            IsShow: AuthService.CheckPermissionsModuleWise('Attendance')
        } ,{
            label: "Report",
            to: AuthService.getMultipleModulePagePath(['Report'],['MonthwiseRpt','ExpenseRpt','MonthwiseSummaryRpt','DivisionWiseSummaryRpt','DepartmentWiseSummaryRpt','DesignationWiseSummaryRpt'],'MonthwiseRpt', 'report'),
            icon: "pi-th-large",
            IsFrontMenu: true,
            IsShow: AuthService.CheckPermissionsModuleWise('Report')
        } ,
        {
            label: "Admin Tools",
            to: AuthService.getMultipleModulePagePath(['AdminTools'],['User','Role','EmailTemplate','MailQueue','AuditTrail','SystemSetting'],'User', 'admintools'),
            icon: "pi-th-large",
            IsFrontMenu: true,
            IsShow: AuthService.CheckPermissionsModuleWise('AdminTools')
        } 
    ]);

    const routes = useRoute();
    watch(() => routes.fullPath, () => {
        emits('sideNavClose',false);
    });
    
</script>
