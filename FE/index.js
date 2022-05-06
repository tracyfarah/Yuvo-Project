$(document).ready(function () {
    //grid data source
    let filter = "day";
    var gridDataSource = new kendo.data.DataSource(
        {
            dataSource: {
            },
            transport: {
                read: {
                    url: `https://localhost:44340/api/Data/get-data-by-day`,
                    dataType: "json"
                },
                schema: {
                    model: {
                        fields: {
                            time: { type: "date" },
                            link: { type: "string" },
                            neAlias: { type: "string" },
                            neType: { type: "string" },
                            max_RX_Level: { type: "number" },
                            max_TX_Level: { type: "number" },
                            rsL_Deviation: { type: "number" },
                        }
                    }
                },
                pageSize: 10,
                sort: {
                    field: "time",
                    dir: "desc"
                }
            }
        });

    //Kendo Chart
    $("#chart").kendoChart({
        dataSource: {
            data: [],
            sort: {
                field: "link",
                dir: "asc"
            }
        },
        title: {
            text: "KPIs",
            font: "20px sans-serif",
            color: "#1a5ef0"
        },
        seriesDefaults: {
            type: "line",
            color: "#1a5ef0"
        },
        series: [{
            name: "MAX_RX_LEVEL",
            field: "max_RX_Level",
            categoryField: "time"
        }],
        seriesClick: function (e) {
            filterGrid(e.category);
        },
        axisLabelClick: function (e) {
            filterGrid(e.value);
        },
        categoryAxis: {
            labels: {
                rotation: -45,
                visual: function (e) {
                    var visual = e.createVisual();
                    visual.options.cursor = "default";
                    return visual;
                }
            }
        },
        valueAxis: {
            title: {
                text: "KPI values"
            },
            labels: {
                format: "{0:n0}"
            }
        },
        tooltip: {
            visible: true,
            template: "#= value #"
        }
    });

    function filterGrid(val) {
        $("#grid").data("kendoGrid").dataSource.filter({
            field: "time",
            operator: "eq",
            value: val
        });
    }

    $("#clearFilter").on("click", function (e) {
        $("#grid").data("kendoGrid").dataSource.filter({});
    });

    $("#dateTimePickerFrom").kendoDateTimePicker({
        value: new Date("2020-03-11 00:00:00"),
        format: "yyyy-MM-dd HH:mm:ss"
    });

    $("#dateTimePickerTo").kendoDateTimePicker({
        value: new Date("2020-03-12 00:00:00"),
        format: "yyyy-MM-dd HH:mm:ss"
    });

    $("#between_btn").on("click", function () {
        var grid = $("#grid").data("kendoGrid");
        let from_date = $("#dateTimePickerFrom").val();
        let to_date = $("#dateTimePickerTo").val();
        console.log(from_date)
        console.log(to_date)
        let betweenUrl = `https://localhost:44340/api/Data/get-data-between/?filter="${filter}"&from=${from_date}&to=${to_date}`;
        $.getJSON(betweenUrl, function (data) {
            grid.setDataSource(data);
        })
    })


    $("#grid").kendoGrid(
        {
            dataSource: gridDataSource,
            dataBound: function (e) {
                var grid = e.sender,
                    chart = $("#chart").data("kendoChart");

                chart.dataSource.data(grid.dataSource.data());
            },
            columns: [{
                field: "time",
                title: "Time",
                format: "{0:dd MMMM yyyy HH}",
            }, {
                field: "link",
                title: "Link",
            }, {
                field: "neAlias",
                title: "NE Alias",
            }, {
                field: "neType",
                title: "NE Type",
            }, {
                field: "max_RX_Level",
                title: "MAX RX LEVEL",
            }, {
                field: "max_TX_Level",
                title: "MAX TX LEVEL",
            }, {
                field: "rsL_Deviation",
                title: "RSL INPUT POWER",
                format: "{0: 00.0}",
            },],

            toolbar: [{
                name: "GRID"
            }],
            height: 630,
            editable: false,
            sortable: true,
            filterable: true,
            pageable: {
                refresh: true,
                pageSizes: true,
                buttonCount: 5,
                pageSize: 10
            },
        }
    );
    //display daily or hourly data in table or grid on button click
    $("#filter_btn").on("click", function () {
        var grid = $("#grid").data("kendoGrid");
        if (this.innerHTML === "Show Daily") {
            filter = "day"
            this.innerHTML = "Show Hourly"
        } else {
            filter = "hour"
            this.innerHTML = "Show Daily"
        }
        $("#rx_btn").prop("checked", true);
        let filterURL = "https://localhost:44340/api/Data/get-data-by-" + filter;

        grid.dataSource.transport.options.read.url = filterURL;
        grid.dataSource.read();
    });


    const radioButtons = $('input[name="kpi_choice"]');
    for (let radioButton of radioButtons) {
        radioButton.addEventListener('change', function (e) {
            console.log(e);
            if (this.checked) {
                $("#chart").data("kendoChart").options.series[0].field = this.value;
                $("#chart").data("kendoChart").options.series[0].name = this.value.toUpperCase();
                console.log(this.value);
                $("#chart").data("kendoChart").refresh();

            }
        });
    }










});

