# FuseSharp

FuseSharp is a **FUSE** wrapper for **macOS** (GNU/Linux in the future) developed in C# by Códice Software.  
It provides a clean API designed specifically to enable the development of userspace filesystems in .NET applications.  
FuseSharp only uses .NET Standard 2.0 features, so it is highly interoperable with other .NET Platforms such as Core, Mono, and Xamarin.Mac.  

* [Goal](#goal)
  * [Usage scenarios](#usage-scenarios)
* [Setup and usage](#setup-and-usage)
* [Project Overview](#project-overview)
* [Performance tests](#performance-tests)
* [License](#license)

## Goal

The goal of this project is to provide an API which enables C# applications to easily implement a filesystem in user-space. This API is written in C# and solely relies on **.NET Standard 2.0 features**, so it's compatible with the following .NET platforms:

|       Platform | Minimum version |
|---------------:|----------------:|
|      .NET Core |             2.0 |
| .NET Framework |           4.6.1 |
|           Mono |             5.4 |
|    Xamarin.Mac |             3.8 |

### Usage scenarios

If you stumbled upon this repository by accident, you might be wondering _"why would I want / need to build a filesystem in user space?"_ The answer is: a filesystem adds a layer of abstraction that empowers and eases use cases you might not have thought of before.

And because said FS is running on user space, you can let FUSE do the heavy lifting, while you will only need to focus on what really matters development-wise.

Did you know there is a filesystem that lets you watch YouTube videos as if they were locally stored in you machine, without having to navigate to the web portal? Or that you can add a transparent encryption layer to a directory tree, to let your users have a secure storage without having to deal with how files are encrypted and decrypted?

Here are some other FUSE-powered filesystem implementations in the wild that might inspire you (mind that **none** of them were developed using FuseSharp, let us know if you implement one yourself with this library!):

* [**SSHFS**](https://github.com/osxfuse/osxfuse/wiki/SSHFS): Mounts a remote directory tree through a SSH connection.
* [**EXT**](https://github.com/osxfuse/osxfuse/wiki/Ext): Provides [EXT filesystem](https://en.wikipedia.org/wiki/Extended_file_system) support for macOS (read by default, but write can be enabled).
* [**NTFS-3G**](https://github.com/osxfuse/osxfuse/wiki/NTFS-3G): Provides [NTFS filesystem](https://en.wikipedia.org/wiki/NTFS) support for macOS (both read and write).
* [**XFS**](https://github.com/osxfuse/osxfuse/wiki/XFS): Provides [XFS filesystem](https://en.wikipedia.org/wiki/XFS) support for macOS.
* [**procfs**](https://github.com/osxfuse/osxfuse/wiki/procfs): Implements a [procfs filesystem](https://en.wikipedia.org/wiki/Procfs) on macOS, which [is not provided by default](http://osxbook.com/book/bonus/chapter11/procfs/).
* [**Accessibility FS**](https://code.google.com/archive/p/macfuse/wikis/MACFUSE_FS_ACCESSIBILITYFS.wiki): A filesystem that exposes the applications that are running in your computer, and the user interface elements said applications expose (including the properties of this GUI components!).
* [**YouTube FS**](https://code.google.com/archive/p/youtubefs/): YoutubeFS enables you to browse your favorite Youtube videos locally on your desktop without going to the youtube website.
* [**fuse-zip**](https://bitbucket.org/agalanin/fuse-zip): Implements a filesystem to navigate, extract, create and modify ZIP and ZIP64 archives based on libzip implemented in C++, working with ZIP archives as real directories.

## Setup and usage

The project is **not** available in NuGet for now. In order to start using it, you must follow these steps:

1. Clone this repository.
2. Install dependencies:
     * ``glib`` 2.56.1. It is available for macOS users through ``brew``.
     * .NET Core SDK.
     * Apple Developer Tools.
3. Compile and install the adaptor library:
     * Executing the `buildandcopy` script located at `/src/Adaptor`
4. Compile the FuseSharp library and the example application:
     * Executing `dotnet build` at `/src/FuseSharp`, or opening the FuseSharp solution with your IDE of choice and building it.

Once you complete these steps, you can add your `FuseSharp.dll` assembly as a project dependency.  
If you distribute an application that uses FuseSharp, bear in mind that, in order to run the application, **the target machine needs to have installed the ``Adaptor.dylib`` library** compiled at step 3.

### Example application

You can also browse and play with the example application located at `src/FuseSharp/example`.  
The application is self-explanatory if executed without arguments.

It implements two different User-Space Filesystems:

* Mirror: it creates a mirror FS, mounted at the specified path, mirroring the content of the root path specified.
* Encrypted: it creates an encrypted FS, mounted at the specified path, mirroring and encrypting/decrypting the content of the root path specified on the fly.

To implement your own filesystem, you need to subclass the ``FileSystem`` type, overriding the necessary methods. You can browse the example to see how. Here's a little GIF demonstrating how it works:

![FuseSharp demo demonstration](https://raw.githubusercontent.com/PlasticSCM/FuseSharp/master/img/demo.gif)

Once you have finished your filesystem, mounting it is as easy as this:

```
using (MyOwnFileSystemImpl fs = new MyOwnFileSystemImpl(targetRoot))
using (FileSystemHandler fsh = new FileSystemHandler(fs, args))
{
    int result = fsh.start();
}
```

### FileSystemHandler arguments

The arguments used to instantiate a `FileSystemHandler` instance are, in the end, passed down to FUSE. FUSE's arguments documentation is sometimes hard to find, but we have collected and tested the following (apart from the [usual args](https://github.com/osxfuse/osxfuse/wiki/Mount-options)):

| Option | Description | Source |
|--------|-------------|--------|
| ``big_writes`` | FUSE driver option - enabled max_write | [Question about write buffer size](http://fuse.996288.n3.nabble.com/Question-about-write-buffer-size-td13137.html) |
| ``max_write`` | Libfuse option. Max write size in bytes | [Question about write buffer size](http://fuse.996288.n3.nabble.com/Question-about-write-buffer-size-td13137.html) and [To FUSE or not to FUSE](https://www.usenix.org/system/files/conference/fast17/fast17-vangoor.pdf) |
| ``writeback_cache`` | Libfuse option | [Question about write buffer size](http://fuse.996288.n3.nabble.com/Question-about-write-buffer-size-td13137.html) and [To FUSE or not to FUSE](https://www.usenix.org/system/files/conference/fast17/fast17-vangoor.pdf) |
| ``splice_read``, ``splice_write``, ``splice_move`` | Activate splicing for all operations | [To FUSE or not to FUSE](https://www.usenix.org/system/files/conference/fast17/fast17-vangoor.pdf) |
| ``direct_io`` | Some file systems may not know the sizes of files that they provide. This could be because a file's content is being streamed so it's difficult or impossible to know the "size" of the file. The content could be dynamically changing so it may not make sense to advertise a size at getattr time only to find that the size has changed at read or write time. procfs is a good example of a osxfuse file system with such needs. What these file systems would like is to be able to allow reads and writes without the file size mattering. This isn't normally possible in the normal I/O paths in the kernel. In particular, short reads from a user-space file system will be zero filled by osxfuse. The ``direct_io`` option causes osxfuse to use an alternative "direct" I/O path between the kernel and the user-space file system. This path makes the file size irrelevant--a read will go on until the file system keeps returning data. There is also no automatic zero filling. In particular, as an implementation side effect, the I/O path bypasses the unified buffer cache altogether. ``direct_io`` is a rather abnormal mode of operation from Mac OS X's standpoint. Unless your file system requires this mode, I wouldn't recommend using this option. | [osxfuse mount options](https://github.com/osxfuse/osxfuse/wiki/Mount-options) |
| ``iosize`` | ``iosize=N``, where N is the I/O size in bytes. You can use this option to specify the I/O size osxfuse should use while accessing the hypothetical storage device corresponding to a osxfuse volume. The minimum possible I/O size is 512 bytes, whereas the largest is 32MB. The size must also be a power of 2. | [osxfuse mount options](https://github.com/osxfuse/osxfuse/wiki/Mount-options) |
| ``auto_xattr`` | By default, osxfuse provides a flexible and adaptive mechanism to handle extended attributes (including things such as Finder Info, Resource Forks, and ACLs). It will initially forward extended attributes calls up to the user-space file system. If the latter does not implement extended attribute functions, osxfuse will remember this and will not forward subsequent calls. It will store extended attributes as Apple Double (`._`) files. If the user-space file system does implement extended attribute functions, it can choose to handle all or only some extended attributes. If there are certain extended attributes that the user-space file system wants osxfuse (the kernel) to handle through `._` files, it should return ENOTSUP for such attributes. The ``auto_xattr`` option tells osxfuse to not bother with sending any extended attributes calls up to user-space, regardless of whether the user-space file system implements the relevant functions or not. With ``auto_xattr``, the kernel will _ALWAYS_ use `._` files. | [osxfuse mount options](https://github.com/osxfuse/osxfuse/wiki/Mount-options) |


## Project Overview

Filesystems in macOS live in the kernel, and as such any modifications would require kernel extensions. To avoid this, FUSE (Filesystem in USErspace) was developed. It allows userspace programs to supply a filesystem to the kernel. The developer is thus free to implement the filesystem however they wish.

The FUSE project consists of two components: the FUSE kernel module, and the LIBFUSE userspace library. LIBFUSE provides the reference implementation for communicating with the FUSE kernel module.

A FUSE filesystem is typically implemented as a standalone application tthat links with LIBFUSE. The later provides functions to mount the filesystem, unmount it, read requests from the kernel, and send responses back.

In addition to registering a new filesystem, FUSE's kernel module also registers a `/dev/fuse` block device. This device serves as an interface between user-space FUSE daemons and the kernel. In general, daemon reads FUSE requests from `/dev/fuse`, processes them, and then writes replies back to `/dev/fuse`.

![FuseSharp architecture overview](https://raw.githubusercontent.com/PlasticSCM/FuseSharp/master/img/architecture.png)

The figure shows FUSE's along with FuseSharp high-level architecture. When a user application performs some operation on a mounted FUSE filesystem, the VFS routes the operation to FUSE's kernel driver.

The driver then allocates a FUSE request structure and puts it in a FUSE queue. At this point, the process that submitted the operation is usually put in a wait state. FUSE's user-space daemon then picks the request from the kernel queue by reading from `/dev/fuse` and processes the request. Processing the request might require entering the kernel again: for example, in case of a stackable FUSE filesystem, the daemon submits the operations to the underlying filesystem (e.g., EXT4); or in case of a block-based FUSE filesystem, the daemon reads or writes from the block device. When done with processing the request, the FUSE daemon writes the response back to `/dev/fuse`; FUSE's kernel driver then marks the request as completed and wakes up the original user process.

Some filesystem operations invoked by an application can complete without communicating with the user-space FUSE daemon. For example, reads from a file whose pages are caches in the kernel page cache, do not need to be forwarded to the FUSE driver.

FuseSharp also consists of two components. An ``Adaptor.dylib`` library that wraps calls to the libfuse library, and the FuseSharp DLL itself, which communicates with the former through a set of callbacks passed down using the `pinvoke` mechanism.

We took this aproach because we found that the libfuse library is complicated and very poorly documented (the code is the documentation, but the complexity of the code makes it difficult to easily understand its intention). This Adaptor library, written in C, implements the key steps of setting up a communication channel with the FUSE kernel, and the processing loop, in a simpler way that handling all at once in .NET space.

FuseSharp leverages modern .NET approaches to file handling, threading and memory management, and exposes a modern API designed specifically to enable the development of userspace filesystems, through a simple architecture implemented solely using .NET Standard.

## Performance tests

Here you can compare FuseSharp performance against the equivalent C and native HFS+ operations. This is still untested against APFS.

All of the timings are measured in seconds.  
Each value is the average after executing the same test case three times.  
Each test was executed:

* After clearing file cache, using the `purge` command.
* With Spotlight filesystem indexing disabled.
* With automated backups disabled.

### Sequential writes

Sequential write requests of 100 files of 32KB each.

| Requests |    C# |     C | Native HFS+ |
|---------:|------:|------:|------------:|
|      100 | 0,683 | 0,753 |       0,633 |

### Concurrent writes

N concurrent write requests of files of 32KB each.

| Requests |    C# |     C | Native HFS+ |
|---------:|------:|------:|------------:|
|        1 | 0,026 | 0,022 |       0,020 |
|       10 | 0,068 | 0,071 |       0,052 |
|       50 | 0,262 | 0,284 |       0,190 |
|      100 | 0,552 | 0,553 |       0,365 |

![Concurrent writes performance graph | FuseSharp](https://raw.githubusercontent.com/PlasticSCM/FuseSharp/master/img/write-test-graph2.png "Concurrent writes performance graph")

### Big file writes

Write request of a file.

| File size |   C# | Native HFS+ |
|----------:|-----:|------------:|
|       1GB | 17,8 |         4,5 |

### Sequential reads

Sequential read requests of 100 files of 32KB each.

| Requests |    C# |     C | Native HFS+ |
|---------:|------:|------:|------------:|
|      100 | 0,695 | 0,632 |       0,519 |

### Concurrent reads

N concurrent read requests of files of 32KB each.

| Requests |    C# |     C | Native HFS+ |
|---------:|------:|------:|------------:|
|        1 | 0,085 | 0,056 |       0,057 |
|       10 | 0,105 | 0,080 |       0,070 |
|       50 | 0,271 | 0,229 |       0,182 |
|      100 | 0,482 | 0,426 |       0,326 |

![Concurrent reads performance graph | FuseSharp](https://raw.githubusercontent.com/PlasticSCM/FuseSharp/master/img/read-test-graph2.png "Concurrent reads performance graph")

### Big file reads

Read requests of a file.

| File size |   C# | Native HFS+ |
|----------:|-----:|------------:|
|       1GB | 14,8 |         8,6 |

## License

MIT License

Copyright (c) 2019 Códice Sofware S.L.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
