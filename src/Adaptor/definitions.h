
#ifndef definitions_h
#define definitions_h

#include <glib.h>

G_BEGIN_DECLS

/*
 * Managed Structure Declarations
 */

struct NetFuse_Args;
struct NetFuse_OpenedPathInfo;
struct NetFuse_Operations;

/*
 * Inferred Structure Declarations
 */

struct fuse_args;

/*
 * Delegate Declarations
 */

typedef int (*AccessPathCb) (const char* path, int mode);
typedef int (*ChangePathOwnerCb) (const char* path, gint64 owner, gint64 group);
typedef int (*ChangePathPermissionsCb) (const char* path, unsigned int mode);
typedef int (*ChangePathTimesCb) (const char* path, void* buf);
typedef int (*CreateDirectoryCb) (const char* path, unsigned int mode);
typedef int (*CreateHandleCb) (const char* path, unsigned int mode, void* info);
typedef int (*CreateHardLinkCb) (const char* oldpath, const char* newpath);
typedef int (*CreateSpecialFileCb) (const char* path, unsigned int perms, guint64 dev);
typedef int (*CreateSymbolicLinkCb) (const char* oldpath, const char* newpath);
typedef void (*DestroyCb) (void* conn);
typedef int (*FlushHandleCb) (const char* path, void* info);
typedef int (*GetFileSystemStatusCb) (const char* path, void* buf);
typedef int (*GetHandleStatusCb) (const char* path, void* buf, void* info);
typedef int (*GetPathExtendedAttributeCb) (const char* path, const char* name, unsigned char* value, guint64 size, int* bytesWritten);
typedef int (*GetPathStatusCb) (const char* path, void* stat);
typedef void* (*InitCb) (void* conn);
typedef int (*ListPathExtendedAttributesCb) (const char* path, unsigned char* list, guint64 size, int* bytesWritten);
typedef int (*LockHandleCb) (const char* path, void* info, int cmd, void* flockp);
typedef int (*MapPathLogicalToPhysicalIndexCb) (const char* path, guint64 logical, guint64* physical);
typedef int (*OpenDirectoryCb) (const char* path, void* info);
typedef int (*OpenHandleCb) (const char* path, void* info);
typedef int (*ReadDirectoryCb) (const char* path, void* buf, void* filler, gint64 offset, void* info, void* stbuf);
typedef int (*ReadHandleCb) (const char* path, unsigned char* buf, guint64 size, gint64 offset, void* info, int* bytesRead);
typedef int (*ReadSymbolicLinkCb) (const char* path, void* buf, guint64 bufsize);
typedef int (*ReleaseDirectoryCb) (const char* path, void* info);
typedef int (*ReleaseHandleCb) (const char* path, void* info);
typedef int (*RemoveDirectoryCb) (const char* path);
typedef int (*RemoveFileCb) (const char* path);
typedef int (*RemovePathExtendedAttributeCb) (const char* path, const char* name);
typedef int (*RenamePathCb) (const char* oldpath, const char* newpath);
typedef int (*RenameXPathCb) (const char* oldpath, const char* newpath, unsigned int flags);
typedef int (*SetPathExtendedAttributeCb) (const char* path, const char* name, unsigned char* value, guint64 size, int flags);
typedef int (*SynchronizeDirectoryCb) (const char* path, int onlyUserData, void* info);
typedef int (*SynchronizeHandleCb) (const char* path, int onlyUserData, void* info);
typedef int (*TruncateFileb) (const char* path, gint64 length);
typedef int (*TruncateHandleCb) (const char* path, gint64 length, void* info);
typedef int (*WriteHandleCb) (const char* path, unsigned char* buf, guint64 size, gint64 offset, void* info, int* bytesWritten);

/*
 * Structures
 */

struct NetFuse_Args {
	int   argc;
	void* argv;
	int   allocated;
};

int NetFuseArgsToArgs (struct NetFuse_Args* from, struct fuse_args *to);
int ArgsToNetFuseArgs (struct fuse_args *from, struct NetFuse_Args* to);


struct NetFuse_OpenedPathInfo {
	int     flags;
	int     write_page;
	int     direct_io;
	int     keep_cache;
	guint64 file_handle;
};

struct NetFuse_Operations {
	GetPathStatusCb                 getattr;
	ReadSymbolicLinkCb              readlink;
	CreateSpecialFileCb             mknod;
	CreateDirectoryCb               mkdir;
	RemoveFileCb                    unlink;
	RemoveDirectoryCb               rmdir;
	CreateSymbolicLinkCb            symlink;
	RenamePathCb                    rename;
	RenameXPathCb                   renamex;
	CreateHardLinkCb                link;
	ChangePathPermissionsCb         chmod;
	ChangePathOwnerCb               chown;
	TruncateFileb                   truncate;
	ChangePathTimesCb               utime;
	OpenHandleCb                    open;
	ReadHandleCb                    read;
	WriteHandleCb                   write;
	GetFileSystemStatusCb           statfs;
	FlushHandleCb                   flush;
	ReleaseHandleCb                 release;
	SynchronizeHandleCb             fsync;
	SetPathExtendedAttributeCb      setxattr;
	GetPathExtendedAttributeCb      getxattr;
	ListPathExtendedAttributesCb    listxattr;
	RemovePathExtendedAttributeCb   removexattr;
	OpenDirectoryCb                 opendir;
	ReadDirectoryCb                 readdir;
	ReleaseDirectoryCb              releasedir;
	SynchronizeDirectoryCb          fsyncdir;
	InitCb                          init;
	DestroyCb                       destroy;
	AccessPathCb                    access;
	CreateHandleCb                  create;
	TruncateHandleCb                ftruncate;
	GetHandleStatusCb               fgetattr;
	LockHandleCb                    lock;
	MapPathLogicalToPhysicalIndexCb bmap;
};


G_END_DECLS

#endif 

