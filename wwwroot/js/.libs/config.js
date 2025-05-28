/**
 * Config
 * -------------------------------------------------------------------------------------
 * ! IMPORTANT: Make sure you clear the browser local storage In order to see the config changes in the template.
 * ! To clear local storage: (https://www.leadshook.com/help/how-to-clear-local-storage-in-google-chrome-browser/).
 */
'use strict';
// JS global variables
let _namespace = 'kanban';
let _hostname_dev = 'http://localhost:7277';
let _hostname_prod = 'http://hmmta-tpcap/kanban';
const queryString = window.location.search;
//console.log('queryString >> ' + queryString);
const _NAMESPACE_ = (window.location.host.indexOf('localhost') >= 0 ? '' : 'kanban');
const _HOSTNAME_ = (_NAMESPACE_ == '' ? _hostname_dev : _hostname_prod);
const _DEV_ = (_NAMESPACE_ == '' ? true : (queryString == '?dev' ? true : false));
const _SYSTEMNAME_ = 'Hino Kanban F.3';
const _DEVELOPER_ = {
    'domain': 'THI_DM_1',
    'username': '20223983',
    'devicename': 'HM22-44-013',
    'fulldevicename': 'HM22-44-013.hmmt.co.th',
    'ipaddress': '172.20.5.3'
};
const _STORAGESERVER_ = _HOSTNAME_ + '/Storage/DownloadTemp';
const _REPORTINGSERVER_ = 'http://hmmt-app03/Reports/Pages/ReportViewer.aspx?';
const _LOV_ = {};
