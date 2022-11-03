# NRSRx Logging Splunk Provider

[![Nuget Package](https://img.shields.io/nuget/v/IkeMtz.NRSRx.Logging.Splunk.svg)](https://www.nuget.org/packages/IkeMtz.NRSRx.Logging.Splunk) ![](https://img.shields.io/nuget/dt/IkeMtz.NRSRx.Logging.Splunk)

Provides Splunk Http Event Collector (HEC) logging to NRSRx based services.

## Configuration Options:
| Name                          | Required | Description                                                                                                                 |
| ----------------------------- | -------- | --------------------------------------------------------------------------------------------------------------------------- |
| SPLUNK_HOST                   | ✅        | The host url of the Splunk server (ie: http://localhost:8000)                                                               |
| SPLUNK_TOKEN                  | ✅        | The token used to authenticate to the the SPLUNK HEC                                                                        |
| SPLUNK_URI_PATH               |          | The uri path of the Splunk HEC (default value: 'services/collector/event')                                                  |
| SPLUNK_INDEX                  |          | The Splunk index name for your logs (default value: '').                                                                    |
| SPLUNK_SOURCE_TYPE            |          | The source type of your logs (default value: '').                                                                           |
| ENVIRONMENT_NAME              |          | The host name used in your logs.  Typically when you want to set this to the name of your microservice. (default value: '') |
| SPLUNK_DISABLE_SSL_VALIDATION |          | Set to true if you want to skip SSL validation when communicating with your Splunk server (default value: false)            |