<template>
  <div class="common__Table mt-5">
    <div class="admin-actions-btn-wrapper mt-0 mb-0">
      <div class="left-part">
        <common-button
          class="btn p-button-aux mr-2"
          :label="'Export to Excel'"
          v-show="resultData.length > 0"
          @click="exportData(ExportType.Excel)"
        />
      </div>
      <div class="right-part"></div>
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
        <!-- MULTI-LEVEL HEADER -->
        <template #header>
    <ColumnGroup>
       <Row>
              <th rowspan="2">Designation</th>
              <th
                v-for="group in groupedColumns"
                :key="group.group"
                :colspan="group.children.length"
                class="text-center"
              >
                {{ group.group }}
              </th>
            </Row>
    </ColumnGroup>
  </template>

  <Column field="Designation" header="Designation" />

  <template v-for="group in groupedColumns" :key="group.group + '-cols'">
    <Column
      v-for="child in group.children"
      :key="child.field"
      :field="child.field"
      :header="child.header"
      class="text-center"
      :body="formatCell(child.field)"
      :footer="formatFooter(child.field)"
      footerStyle="font-weight: bold; background: #f5f5f5"
    />
  </template>

        <!-- Empty Template -->
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
import UrlConstants from "@/utils/urlconstants";
import { ExportType } from "@/models/controls/Grid/gridRequest";
import GridConfig from "@/models/controls/Grid/gridConfig";
import moment from "moment";

const { PatchData, PostData, ExportData } = new commonModule();

const dt = ref(null);
const resultData = ref([]);
const footerRow = ref<any>({});
const ResultSelected = ref(null);

// Grouped column definition
const groupedColumns = ref<any[]>([]);

// Initial config
const gridConfig = ref({
  api: '',
  deleteApi: '',
  filters: [],
  sortcolumn: "Id",
  sortorder: -1,
  doubleClickHander: null,
} as GridConfig);

// Load data
onMounted(async () => {
  await loadData();
});

const loadData = async () => {
  PostData(
    UrlConstants.apiGetDesignationWiseSummaryRpt,
    gridConfig.value,
    "",
    (response: any) => {
        debugger
      const allRows = response;
      footerRow.value = allRows.find((row: any) => row.Designation === "Grand Total") || {};
      resultData.value = allRows.filter((row: any) => row.Designation !== "Grand Total");

      // Extract month keys like "Feb-25_Days", "Feb-25_Amt"
      const keys = Object.keys(resultData.value[0] || {}).filter(k => k !== "Designation");

      const monthGroups: Record<string, { field: string, header: string }[]> = {};

      keys.forEach(key => {
        const [month, type] = key.split("_");
        if (!monthGroups[month]) monthGroups[month] = [];
        monthGroups[month].push({ field: key, header: type === "Days" ? "Days" : "Amount" });
      });

      // Convert to array for rendering
      groupedColumns.value = Object.entries(monthGroups).map(([group, children]) => ({
        group,
        children
      }));
    }
  );
};

// Format cell values
const formatCell = (field: string) => {
  return (rowData: any) => {
    const value = rowData[field];
    return typeof value === "number" ? value.toLocaleString("en-IN") : value;
  };
};

// Format footer values
const formatFooter = (field: string) => {
  const value = footerRow.value[field];
  return typeof value === "number" ? value.toLocaleString("en-IN") : value;
};

// Export logic
const exportData = (exportType: any) => {
  const postData: any = {
    ResponseType: exportType,
    Columns: [],
  };

  groupedColumns.value.forEach(group => {
    group.children.forEach(child => {
      postData.Columns.push({
        name: `${group.group} - ${child.header}`,
        data: child.field,
      });
    });
  });

  ExportData(
    UrlConstants.apiGetDesignationWiseSummaryRpt,
    postData,
    "Designation_Summary_" + moment().utcOffset(0, true).format("YY_MMM_DD_hhmmss A"),
    "",
    exportType === 1 ? "xlsx" : "pdf",
    () => {}
  );
};
</script>
