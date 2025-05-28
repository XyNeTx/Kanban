var P = "TEST";
var Q = "TEST2";
var R = "TEST3";
function testFunction(): Promise<string> {
    let result: string = P + " " + Q + " " + R;
    console.log(result);
    return new Promise((resolve) => {
        resolve(result);
    });
}
window.addEventListener("load", async () => {
    console.log("Test script loaded");
    const output = await testFunction();
    console.log("Function returned:", output);
});