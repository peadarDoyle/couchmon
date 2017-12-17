# CouchMon
Provides basic health monitoring for a couchbase cluster; it is a plugin for the Nimator framework. The solution contains the CouchMon, SampleApp and Tests project. The SampleApp and Test projects should be self explanatory.

# Running the Sample App
To run the sample app you need to have access to a Couchbase cluster either locally installed or on a remote machine. Before running the sample app you need to update application settings - including the coucbase Username and Password as well as the ips of the nodes in the cluster.

# Monitors
Currently CouchMon montiors document and RAM usage of Buckets. It uses a Threshold value (e.g. 85% of RAM) which once breached results in a warning notification.

# Expanding Monitoring
Bucket RAM and Document usage are important but the following is further information worth tracking.

* Cache vs Disk Reads (ratio)
* Node RAM usage
* CPU usage
* Swap memroy

See https://developer.couchbase.com/documentation/server/current/monitoring/monitoring-rest.html for more information on what to monitor.

# An Brief introduction to the CouchMon source code
The code base is quite straight forward if you're familiar with how Nimator works. There are two Checks which check all buckets in a cluster for both high RAM and Document usage. Bucket check have a basse BucketCheck class.

There are three aspects of the code that are worth mentioning. The IClusterService, CouchMonModule, and CouchMonContext.

IClusterService is a service that accesses the Couchbase Cluster API to get Node/Bucket info. This service was built to be loosley coupled and is easily tested since we can inject dependencies to mock the HTTP request/response. All instances of IClusterService should share the same ICluster instances since this is an expensive resource.

CouchMonModule is an AutoFac (a .NET DI Framework) module. This module wires up the IClusterService dependencies. It access application settings to when necessary. 

CouchMonContext was created primarily so that we can share the ICluster instance throughout the plugin. Using the GetInstance<T> method we create new instances of IClusterService. It is important to create new instances this way since the CouchMon module resolves ICluster to the same instance regardless of how many instances of IClusterService uses it.
