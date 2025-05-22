import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import { defineStore } from "pinia";
// import { AuthService } from '@/composables/api/authService'
// import { useToastStore } from "@/store/useToastStore";
import { useAuthDataStore } from "@/store/useAuthDataStore";
import { AuthService } from "@/composables/api/authService";

const routes: Array<RouteRecordRaw> = [
  {
    path: "/notfound",
    name: "error404",
    component: () => import("@/views/errors/404.vue"),
    meta: { name: "Page not found", requiresAuth: false },
  },
  {
    path: "/:pathMatch(.*)*",
    name: "NotFound",
    component: () => import("@/views/errors/404.vue"),
    redirect: { name: "error404" },
    children: [],
  },
  {
    path: "/unauthorized",
    name: "unauthorized",
    component: () => import("@/views/errors/401.vue"),
    meta: { name: "unauthorized", requiresAuth: false },
  },
  {
    path: "/forbidden",
    name: "forbidden",
    component: () => import("@/views/errors/403.vue"),
    meta: { name: "forbidden", requiresAuth: false },
  },
  {
    path: "/",
    name: "Login",
    component: () => import("@/login.vue"),
    meta: { title: "", name: "Login", requiresAuth: false },
  },
  {
    path: "/Login/ResetPassword",
    name: "Reset Password",
    component: () => import("@/resetPassword.vue"),
    meta: { title: "", name: "Reset Password", requiresAuth: false },
    props: (route) => ({ Id: route.query.Id }),
  },
  {
    path: "/home",
    name: "Home",
    components: {
      default: () => import("@/home.vue"),
      layout: () => import("@/components/shared/layout/defaultLayout.vue"),
    },
    meta: { title: "", name: "Home", requiresAuth: true, MainPage: "Home" },
    redirect: { name: "Dashboard" },
    children: [
      {
        path: "/dashboard",
        name: "Dashboard",
        component: () => import("@/views/frontEnd/dashboard.vue"),
        meta: {
          title: "",
          name: "Dashboard",
          requiresAuth: false,
          MainPage: "Dashboard",
        },
      },
      {
        path: "/master",
        name: "Master",
        component: () => import("@/views/frontEnd/master.vue"),
        meta: {
          title: "EMS | Master",
          name: "Master",
          requiresAuth: true,
          TabName: "master",
          MainPage: "Master",
          
        },
        redirect: { name: "Employee" },
        children: [
          
          {
            path: "/master/employee",
            name: "Employee",
            component: () =>
              import("@/views/frontEnd/master/employee/employee-list.vue"),
            meta: {
              title: "EMS | Master | Employee",
              name: "Employee",
              requiresAuth: true,
              children: [],
              MainPage:'Master',
              TabName:'master',              
              ModuleName: ['Masters'] ,
              PageCode:['Employee']
            },
          },
          
        ],
      },
      {
        path: "/admintools",
        name: "AdminTools",
        component: () => import("@/views/frontEnd/admintools.vue"),
        meta: {
          title: "EMS | Admin Tools",
          name: "AdminTools",
          requiresAuth: true,
          TabName: "admintools",
          MainPage: "AdminTools",          
        },
        redirect: { name: 'User' },
        children: [
          {
            path: "/admintools/users",
            name: "User",
            component: () =>
              import("@/views/frontEnd/admintools/users/user-list.vue"),
            meta: {
              title: "",
              name: "User",
              requiresAuth: true,
              MainPage:'AdminTools',
              TabName:'admintools',              
              ModuleName: ['AdminTools'] ,
              PageCode:['User']
            },
          },
          {
            path: "/admintools/role",
            name: "Role",
            component: () =>
              import("@/views/frontEnd/admintools/roles/roles-list.vue"),
            meta: {
              title: "",
              name: "Role",
              requiresAuth: true,
              MainPage:'AdminTools',
              TabName:'admintools', 
              ModuleName: ['AdminTools'] ,
              PageCode:['Role']
            },
          },
          {
            path: "/admintools/system-setting",
            name: "SystemSetting",
            component: () =>
              import(
                "@/views/frontEnd/admintools/websetting/websiteSettings.vue"
              ),
            meta: {
              title: "",
              name: "SystemSetting",
              requiresAuth: true,
              MainPage:'AdminTools',
              TabName:'admintools', 
              ModuleName: ['AdminTools'] ,
              PageCode:['SystemSetting']
            },
          },
          {
            path: "/admintools/email-template",
            name: "EmailTemplate",
            component: () =>
              import(
                "@/views/frontEnd/admintools/emailtemplates/emailTemplateList.vue"
              ),
            meta: {
              title: "",
              name: "EmailTemplate",
              requiresAuth: true,
              MainPage:'AdminTools',
              TabName:'admintools', 
              ModuleName: ['AdminTools'] ,
              PageCode:['EmailTemplate']
            },
          },
          {
            path: "/admintools/mail-list",
            name: "MailList",
            component: () =>
              import("@/views/frontEnd/admintools/mailList/mail-Queue.vue"),
            meta: {
              title: "",
              name: "MailList",
              requiresAuth: true,
              MainPage:'AdminTools',
              TabName:'admintools', 
              ModuleName: ['AdminTools'] ,
              PageCode:['MailQueue']             
            },
          },
          {
            path: "/admintools/audit-logs",
            name: "AuditLogs",
            component: () =>
              import("@/views/frontEnd/admintools/auditLog/audit-log.vue"),
            meta: {
              title: "",
              name: "AuditLogs",
              requiresAuth: true,
              MainPage:'AdminTools',
              TabName:'admintools', 
              ModuleName: ['AdminTools'] ,
              PageCode:['AuditTrail']
            },
          },
        ],
      },    
      {
        path: "/activation",
        name: "Activation",
        component: () => import("@/views/frontEnd/activation.vue"),
        meta: {
          title: "EMS | Activation",
          name: "Activation",
          requiresAuth: true                    
        },
        redirect: { name: "AttendanceActivationList" },
        children: [
          {
            path: "/activation/activation-list",
            name: "AttendanceActivationList",
            component: () => import("@/views/frontEnd/attendance/activation-list.vue"),
            meta: {
              title: "",
              name: "AttendanceActivationList",
              requiresAuth: true,
              MainPage:'Activation',
              TabName:'activation',              
              ModuleName: ['Attendance'] ,
              PageCode:['Activation']
            },
          },
        ],
      },{
        path: "/attendance",
        name: "Attendance",
        component: () => import("@/views/frontEnd/attendance.vue"),
        meta: {
          title: "EMS | Attendance",
          name: "Attendance",
          requiresAuth: true                    
        },
        redirect: { name: "AttendanceList" },
        children: [
          {
            path: "/attendance/attendance-list",
            name: "AttendanceList",
            component: () => import("@/views/frontEnd/attendance/attendance-list.vue"),
            meta: {
              title: "",
              name: "AttendanceList",
              requiresAuth: true,
              MainPage:'Attendance',
              TabName:'attendance',              
              ModuleName: ['Attendance'] ,
              PageCode:['Attendance']
            },
          },
        ],
      },
      {
        path: "/expense",
        name: "Expense",
        component: () => import("@/views/frontEnd/expense.vue"),
        meta: {
          title: "EMS | Expense",
          name: "Expense",
          requiresAuth: true                    
        },
        redirect: { name: "expenseList" },
        children: [
          {
            path: "/expense/expense-list",
            name: "expenseList",
            component: () => import("@/views/frontEnd/expense/expense-list.vue"),
            meta: {
              title: "",
              name: "expenseList",
              requiresAuth: true,
              MainPage:'Expense',
              TabName:'expense',              
              ModuleName: ['Expense'] ,
              PageCode:['Expense']
            },
          }
        ],
      },

      {
        path: "/report",
        name: "report",
        component: () => import("@/views/frontEnd/report.vue"),
        meta: {
          title: "EMS | Report",
          name: "Report",
          requiresAuth: true                    
        },
        redirect: { name: "MonthwiseRptList" },
        children: [
          {
            path: "/report/monthwise-rpt",
            name: "MonthwiseRptList",
            component: () => import("@/views/frontEnd/report/monthwise-rpt.vue"),
            meta: {
              title: "",
              name: "MonthwiseRptList",
              requiresAuth: true,
              MainPage:'Report',
              TabName:'report',              
              ModuleName: ['Report'] ,
              PageCode:['MonthwiseRpt']
            },
          },
          {
            path: "/report/division-summary",
            name: "DivisionWiseRptList",
            component: () => import("@/views/frontEnd/report/divisionWise-rpt.vue"),
            meta: {
              title: "",
              name: "DivisionWiseRptList",
              requiresAuth: true,
              MainPage:'Report',
              TabName:'report',              
              ModuleName: ['Report'] ,
              PageCode:['DivisionWiseSummaryRpt']
            },
          },
          {
            path: "/report/department-summary",
            name: "DepartmentWiseRptList",
            component: () => import("@/views/frontEnd/report/departmentWise-rpt.vue"),
            meta: {
              title: "",
              name: "DepartmentWiseRptList",
              requiresAuth: true,
              MainPage:'Report',
              TabName:'report',              
              ModuleName: ['Report'] ,
              PageCode:['DepartmentWiseSummaryRpt']
            },
          },
          {
            path: "/report/designation-summary",
            name: "DesignationWiseRptList",
            component: () => import("@/views/frontEnd/report/designationWise-rpt.vue"),
            meta: {
              title: "",
              name: "DesignationWiseRptList",
              requiresAuth: true,
              MainPage:'Report',
              TabName:'report',              
              ModuleName: ['Report'] ,
              PageCode:['DesignationWiseSummaryRpt']
            },
          },
          {
            path: "/report/monthwise-summary",
            name: "MonthwiseSummaryRptList",
            component: () => import("@/views/frontEnd/report/summaryReport/monthWise-summaryRpt.vue"),
            meta: {
              title: "",
              name: "MonthwiseSummaryRptList",
              requiresAuth: true,
              MainPage:'Report',
              TabName:'report',              
              ModuleName: ['Report'] ,
              PageCode:['MonthwiseSummaryRpt']
            },
          },
          {
            path: "/report/expense-rpt",
            name: "ExpenseRptList",
            component: () => import("@/views/frontEnd/report/expense-rpt.vue"),
            meta: {
              title: "",
              name: "ExpenseRptList",
              requiresAuth: true,
              MainPage:'Report',
              TabName:'report',              
              ModuleName: ['Report'] ,
              PageCode:['ExpenseRpt']
            },
          },
        ],
      },

    ],
  },
];
const router = createRouter({
  history: createWebHistory(process.env.VUE_APP_BASE_URL),
  routes,
});

router.beforeEach(async (to, from, next) => {
  const authLogin = useAuthDataStore().getAuth as any;
  if (to.meta.requiresAuth) {    
    let prm = true;
    if (
      (to?.meta?.MainPage ?? "") == "Dashboard" &&
      (authLogin?.Token ?? "").length > 0
    ) {
      prm = true;
    }
   
    return (authLogin?.Token ?? "").length == 0
      ? next({ name: "Login", query: { redirect: to.fullPath } })
      : next();
     } else return next();
});

export default router;
