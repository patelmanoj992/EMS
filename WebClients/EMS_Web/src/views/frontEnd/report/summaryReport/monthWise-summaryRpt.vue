<template>
    <div class="mt-5">
    <common-grid :config="gridConfig" ref="dt" :Ispaginator="false" :hideFilter="true" :filters="filters" :filename="filename" :AdvSearchBtnClass="'advance-search-filter-only-for-mobile mt-3'">
        <template v-slot:basicfilter>
            
        </template>
        <template v-slot:advancefilter>
           
        </template>
        <template v-slot:right-buttons>
         </template>
        <template v-slot:columns>
            
            <Column field="SrNo" header="SrNo" class="text-center" :sortable="true"></Column> 
            <Column field="MonthYear" header="Month-Year" class="text-center" :sortable="true"></Column>   
            <Column field="TotalAmountSum" header="Total" class="text-center" :sortable="true"></Column>   
        </template>
    </common-grid>
    </div>
</template>
<script setup lang="ts">
    import GridConfig from "@/models/controls/Grid/gridConfig";
    import SearchOperations from "@/models/controls/Grid/searchOperations";
    import UrlConstants from "@/utils/urlconstants";
    import { inject, ref } from "vue";
    import commonModule from "@/composables/modules/commonModule";
    import { AuthService } from "@/composables/api/authService";
    const Page = AuthService.GetPagePermission('AdminTools','User')
    const { ConvertToUserTimeZone,GetDecimalTwoPlace } = inject('GlobalFunctions');
    const { PatchData } = new commonModule();
    const dt = ref(null);
    const popupManage = ref();
    const ddlItem = ref();
    
   
    const gridConfig = ref({
        api: UrlConstants.apiGetMonthwiseSummaryRpt,
        deleteApi: '',
        filters: [],
        sortcolumn: "Id",
        sortorder: -1,
        doubleClickHander: null,
    } as GridConfig);

       const filename = ref("MonthWise_Summary_"+Date.now())
</script>