let P: string = "TEST";
let Q: string = "TEST2";
let R: string = "TEST3";


function testFunction(): Promise<string> {
    let result: string = P + " " + Q + " " + R;
    //console.log(result);
    return new Promise((resolve) => {
        resolve(result);
    });
}

window.addEventListener("load", async () => {
    //console.log("Test script loaded");
    const output = await testFunction();
    console.log("Function returned:", output);
    await List_Data();
    xSplash.hide();
});


async function List_Data(): Promise<void> {
    await _xLib.AJAX_Get("/api/KBNOR360/List_Data", null,
        function (success: any) {
            success = _xLib.JSONparseMixData(success);
            _xDataTable.ClearAndAddDataDT("#tblMaster", success.data);
        },
        function (error: any) {
            console.error(error);
            //xSwal.xError(error)
        }
    );
}