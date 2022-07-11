# Content Hub Antivirus process

![alt text](https://github.com/rvdplas/content-hub-antivirus/blob/main/Wiki/Content-Hub-Code.png?raw=true)

The Content Hub has Antivirus functionality. Well, that is not entirely true, they support a mechanism to execute an Antivirus flow. To be honest, it's bogus to implement an Antivirus flow ourselves. It should have been available OOTB in my humble opinion. But as it isn't available at this moment, we need to make do with the things that we do have.

So, before we go any further into the details, how should this thing work on high-level. As with many things within the Content Hub, the process start with a user uploading an asset to the Content Hub. Every uploaded asset goes through a media processing pipeline. Based on the asset's file extension it will choose the appropriate pipeline. For each pipeline, an antivirus task needs to be added in order to trigger a virus scan task, more on this later. When the antivirus task has been executed, it will call back a Content Hub URL that was posted with the antivirus request. Based on the result of the antivirus task an asset will be updated with Virus free or not.

![alt text]('https://github.com/rvdplas/content-hub-antivirus/blob/main/Wiki/Antivirus_overview.png?raw=true')

**So what do we need to do to get this working?**
1. Find an online Antivirus provider like Cloudmersive
2. Enable antivirus scanning in your Content Hub instance
3. Extend the Media processing pipeline(s) with the antivirus task*

*** TIP**: make a backup **BEFORE** making changes to your pipeline. The media processing pipelines are tricky. You need to keep in mind that the UI doesn't show you all the options. Make sure to open the pipeline via Entity Management.

