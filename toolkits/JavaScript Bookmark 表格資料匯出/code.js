javascript:(function(){

    const dataCollector = (function() {
        let result = [];
        let ids = new Set();
        
        /* 
         * 合併新的資料列，回傳新增資料筆數
         */
        function mergeDataFromRows(rows) {
            let newData = extractFromRows(rows).filter(x => !ids.has(x.id));
            for (let item of newData) {
                ids.add(item.id);
                result.push(item);
            }
            return newData.length;
        }

        /* 
         * 取得 CSV 內容
         */
        function getCsvContent() {
            return result.map(x => `"${x.id}","${x.title}"`).join('\n');
        }

        /* 
         * 由資料列 DOM 解析資料
         */
        function extractFromRows(rows) {
            return [...rows].map(x => ({
                id: x.querySelector(`div[data-col-index="0"]`)?.textContent?.trim() || '',
                title: x.querySelector(`div[data-testid="property-value"]`)?.textContent?.trim() || ''
            }));
        }

        /* 
         * 檢索資料列 DOM
         */
        function querySelectorForRows() {
            return document.querySelectorAll('.table-view div[data-index]');
        }

        return {
            mergeDataFromRows: mergeDataFromRows,
            getCsvContent: getCsvContent,
            querySelectorForRows: querySelectorForRows
        };
    })();

    function exportCSV(filename, csvContent) {
        let bom = '\uFEFF'; 
        let blob = new Blob([bom + csvContent], { type: 'text/csv;charset=utf-8;' });
        let link = document.createElement('a');
        let url = URL.createObjectURL(blob);
        link.href = url;
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        URL.revokeObjectURL(url);
    }

    (async function() {
        let rows = dataCollector.querySelectorForRows();
        while (rows.length) {
            if (!dataCollector.mergeDataFromRows(rows)) {
                break;
            }

            rows[rows.length - 1].scrollIntoView({ behavior: 'smooth', block: 'start' });
            await new Promise(resolve => setTimeout(resolve, 1000));
            rows = dataCollector.querySelectorForRows();
        }
        let content = dataCollector.getCsvContent();
        if (!content) {
            return alert('無匯出資料');
        }
        exportCSV('result.csv', dataCollector.getCsvContent());
    })();
})();

