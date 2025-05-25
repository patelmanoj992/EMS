<template>
  <div class="common__Table mt-5">
    <div class="admin-actions-btn-wrapper mt-0 mb-0">
            <div class="left-part">
              <common-button class="btn p-button-aux mr-2" :label="'Export to Excel'"
                v-show="resultData.length > 0"   @click="exportData(ExportType.Excel)"  />    
            </div>
            <div class="right-part">
                            
            </div>
        </div>
        <div class="custom-datatablewithoutwidth-wrapper table-responsive">
    <DataTable
      ref="dt"
      :value="resultData"
      :paginator="true"
      class="p-datatable-customers"
      filterDisplay="menu"
      paginatorTemplate="RowsPerPageDropdown CurrentPageReport FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink"
      currentPageReportTemplate="Showing {first} to {last} of {totalRecords}"
      selectionMode="single"
      v-model:selection="ResultSelected"
      :rowsPerPageOptions="[15, 30, 50, 100]"
      :responsiveLayout="'scroll'"
      showGridlines
      :rows="15"
    >
      <!-- Static Column for Department -->
      <Column
        field="Department"
        header="Department"
        :footer="footerRow?.Department"
        footerStyle="font-weight: bold; background: #f5f5f5"
      />

      <!-- Dynamic Columns -->
      <template v-for="col of columnList" :key="col">
        <Column
          :field="col"
          class="text-center"
          :header="col"
          :body="formatCell(col)"
          :footer="formatFooter(col)"
          footerStyle="font-weight: bold; background: #f5f5f5"
        />
      </template>

      <template #empty>
        <div class="no-data">
          <img src="@/assets/images/no-items.png" alt="No Data Found" />
          <h4>No Record Found</h4>
        </div>
      </template>
    </DataTable>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import commonModule from "@/composables/modules/commonModule";
const { PatchData,PostData ,ExportData} = new commonModule();
import UrlConstants from "@/utils/urlconstants";
import { ExportType } from "@/models/controls/Grid/gridRequest";
import GridConfig from "@/models/controls/Grid/gridConfig";
import moment from "moment";
const dt = ref(null);
const resultData = ref([]);
const footerRow = ref<any>({});
const ResultSelected = ref(null);
const columnList = ref<string[]>([]);

const gridConfig = ref({
  api: '',
  deleteApi: '',
  filters: [],
  sortcolumn: "Id",
  sortorder: -1,
  doubleClickHander: null,
} as GridConfig);

onMounted(async () => {
  await loadData();
});

const loadData = async () => {
  PostData(UrlConstants.apiGetDepartmentwiseSummaryRpt, gridConfig.value, "", (response: any) => {
    const allRows = response;
    const lastRow = allRows.find((row: any) => row.Department === "Grand Total");
    const otherRows = allRows.filter((row: any) => row.Department !== "Grand Total");

    resultData.value = otherRows;
    footerRow.value = lastRow || {};

    const keys = Object.keys(otherRows[0] || {});
    columnList.value = keys.filter(k => k !== 'Department');
  }, null);
};

// Cell value formatting
const formatCell = (field: string) => {
  return (rowData: any) => {
    const value = rowData[field];
    return typeof value === 'number' ? value.toLocaleString('en-IN') : value;
  };
};

// Footer value formatting
const formatFooter = (field: string) => {
  const value = footerRow.value[field];
  return typeof value === 'number' ? value.toLocaleString('en-IN') : value;
};


const exportData = (exportType: any) => {


    let Postdata = {} as any
    Postdata = JSON.parse(JSON.stringify({
        ResponseType: exportType,
        Columns: []
    }));
    if ((dt?.value?.columns?.length ?? 0) > 0) {
        for (let i = 0; i < (dt?.value?.columns?.length ?? 0); i++) {
            if (dt?.value?.columns[i]?.props.header != undefined)
                Postdata.Columns.push({
                    name: dt.value?.columns[i]?.props.header,
                    data: dt.value?.columns[i]?.props.field,
                })
        }
    }
    
    ExportData(UrlConstants.apiGetDepartmentwiseSummaryRpt,Postdata, ("Department_Summary_"+(moment(Date.now()).utcOffset(0, true).format("YY_MMM_DD_hhmmss A"))),"",(exportType == 1)?"xlsx":"pdf", (data: any) => {        
                       // gridDataList.value = data;
  });
};
</script>
