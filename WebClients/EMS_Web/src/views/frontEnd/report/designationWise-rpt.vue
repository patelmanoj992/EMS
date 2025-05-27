<template>
  <div class="common__Table mt-5">
    <div class="admin-actions-btn-wrapper mt-0 mb-0">
      <div class="left-part">
        <common-button
          class="btn p-button-aux mr-2"
          :label="'Export to Excel'"
          v-show="sales.length > 0"
          @click="exportData(ExportType.Excel)"
        />
      </div>
      <div class="right-part"></div>
    

  </div>
    <div class="custom-datatablewithoutwidth-wrapper table-responsive">
      <DataTable
        ref="dt"
        :value="sales"
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
    <!-- Dynamic Header -->
    <ColumnGroup type="header">
      <Row>
        <Column header="Designation" :rowspan="2" />
        <template v-for="month in months" :key="month">
          <Column :header="month" :colspan="2" class="text-center" />
        </template>
        <Column header="Total Sum Of Attend Days" :rowspan="2" class="text-center"/>
        <Column header="Total Sum Of Total Amount" :rowspan="2" class="text-center"/>
      </Row>
      <Row>
        <template v-for="month in months" :key="month + '-fields'">
          <Column header="Days" class="text-center"/>
          <Column header="Amount" class="text-center"/>
        </template>
      </Row>
    </ColumnGroup>

    <!-- Table Body -->
    <Column field="Designation" header="Designation" />

    <template v-for="month in months" :key="month + '-body'">
      <Column :field="`${month}_Days`" class="text-right">
        <template #body="slotProps">
          {{ slotProps.data[`${month}_Days`] }}
        </template>
      </Column>
      <Column :field="`${month}_Amt`" class="text-right">
        <template #body="slotProps">
          {{ formatCurrency(slotProps.data[`${month}_Amt`]) }}
        </template>
      </Column>
    </template>

    <Column field="TotalDays" class="text-right">
      <template #body="slotProps">
        {{ slotProps.data.TotalDays }}
      </template>
    </Column>

    <Column field="TotalAmount" class="text-right">
      <template #body="slotProps">
        {{ formatCurrency(slotProps.data.TotalAmount) }}
      </template>
    </Column>

    <!-- Footer Totals -->
    <ColumnGroup type="footer">
      <Row>
        <Column footer="Grand Totals:" :colspan="1" footerStyle="text-align:right" />
        <template v-for="month in months" :key="month + '-footer'">
          <Column :footer="monthDaysTotalMap[month]" class="text-right"/>
          <Column :footer="formatCurrency(monthAmountTotalMap[month])" class="text-right"/>
        </template>
        <Column :footer="grandTotalDays" class="text-right"/>
        <Column :footer="formatCurrency(grandTotalAmount)" class="text-right"/>
      </Row>
    </ColumnGroup>
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
import { ref, computed, onMounted } from 'vue';

import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import ColumnGroup from 'primevue/columngroup';
import Row from 'primevue/row';


import commonModule from "@/composables/modules/commonModule";
import UrlConstants from "@/utils/urlconstants";
import { ExportType } from "@/models/controls/Grid/gridRequest";
import GridConfig from "@/models/controls/Grid/gridConfig";
import moment from "moment";

const { PatchData, PostData, ExportData } = new commonModule();

interface SalesData {
  Designation: string;
  [key: string]: any;
}
const response = ref<SalesData[]>([]);



const months = ref<string[]>([]); // Dynamic month names like "Feb-25", "Mar-25"
const sales = ref<SalesData[]>([]);

// Extract month names from keys
function extractMonths(data: SalesData[]) {
  const keys = Object.keys(data[0]);
  const monthSet = new Set<string>();
  keys.forEach(key => {
    if (key.endsWith('_Days')) {
      const month = key.replace('_Days', '');
      monthSet.add(month);
    }
  });
  return Array.from(monthSet);
}

// Add TotalDays & TotalAmount to each row
function calculateTotals(data: SalesData[], months: string[]) {
  return data.map(row => {
    let totalDays = 0;
    let totalAmount = 0;
    months.forEach(month => {
      totalDays += row[`${month}_Days`] || 0;
      totalAmount += row[`${month}_Amt`] || 0;
    });
    return {
      ...row,
      TotalDays: totalDays,
      TotalAmount: totalAmount,
    };
  });
}

// Month-wise totals
const monthDaysTotalMap = computed(() => {
  const totals: Record<string, number> = {};
  months.value.forEach(month => {
    totals[month] = sales.value.reduce((sum, row) => sum + (row[`${month}_Days`] || 0), 0);
  });
  return totals;
});

const monthAmountTotalMap = computed(() => {
  const totals: Record<string, number> = {};
  months.value.forEach(month => {
    totals[month] = sales.value.reduce((sum, row) => sum + (row[`${month}_Amt`] || 0), 0);
  });
  return totals;
});

// Grand Totals
const grandTotalDays = computed(() =>
  sales.value.reduce((sum, row) => sum + (row.TotalDays || 0), 0)
);
const grandTotalAmount = computed(() =>
  sales.value.reduce((sum, row) => sum + (row.TotalAmount || 0), 0)
);

// Format Currency
function formatCurrency(val: number): string {
  return val?.toLocaleString('en-IN', {
    style: 'currency',
    currency: 'INR',
    minimumFractionDigits: 0
  }) || '';
}

// Init
onMounted(async () => {
  await loadData();

  // months.value = extractMonths(response.value);
  // sales.value = calculateTotals(response.value, months.value);
});

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


const loadData = async () => {
  PostData(
    UrlConstants.apiGetDesignationWiseSummaryRpt,
    gridConfig.value,
    "",
    (response: any) => {
        debugger
      response.value = response;
      months.value = extractMonths(response.value);
  sales.value = calculateTotals(response.value, months.value);
    },null
  );
};



// Export logic
const exportData = (exportType: any) => {
  const postData: any = {
    ResponseType: exportType,
    Columns: [],
    Months: months.value
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
