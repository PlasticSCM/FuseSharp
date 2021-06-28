//
//  adaptor.c
//  Adaptor
//
//  Created by groberts on 15/08/2018.
//  Copyright © 2018 groberts. All rights reserved.
//

#include "adaptor.h"
#include "definitions.h"
#include <fuse.h>
#include <errno.h>
#include <string.h>
#include <stdlib.h>
#include <stdio.h>

#define fprintf(fmt, ...) (0)

static inline struct NetFuse_Operations*
_adaptor_get_private_data()
{
    struct fuse_context* context = fuse_get_context();
    if (!context)
    {
        fprintf(stderr, "%s null context\n", __FUNCTION__);
        return NULL;
    }
    void* private_data = context->private_data;
    return (struct NetFuse_Operations*)private_data;
}

static int
adaptor_getattr (const char *path, struct stat *stat)
{
    return _adaptor_get_private_data()->getattr (path, stat);
}

static int
adaptor_readlink (const char *path, char* buf, size_t size)
{
    return _adaptor_get_private_data()->readlink (path, buf, size);
}

static int
adaptor_mknod (const char *path, mode_t mode, dev_t dev)
{
    return _adaptor_get_private_data()->mknod (path, mode, dev);
}

static int
adaptor_mkdir (const char *path, mode_t mode)
{
    return _adaptor_get_private_data()->mkdir (path, mode);
}

static int
adaptor_unlink (const char *path)
{
    return _adaptor_get_private_data()->unlink (path);
}

static int
adaptor_rmdir (const char *path)
{
    return _adaptor_get_private_data()->rmdir (path);
}

static int
adaptor_symlink (const char *oldpath, const char *newpath)
{
    return _adaptor_get_private_data()->symlink (oldpath, newpath);
}

static int
adaptor_rename (const char *oldpath, const char *newpath)
{
    return _adaptor_get_private_data()->rename (oldpath, newpath);
}

static int
adaptor_renamex (const char *oldpath, const char *newpath, unsigned int flags)
{
    return _adaptor_get_private_data()->renamex (oldpath, newpath, flags);
}

static int
adaptor_link (const char *oldpath, const char *newpath)
{
    return _adaptor_get_private_data()->link (oldpath, newpath);
}

static int
adaptor_chmod (const char *path, mode_t mode)
{
    return _adaptor_get_private_data()->chmod (path, mode);
}

static int
adaptor_chown (const char *path, uid_t uid, gid_t gid)
{
    return _adaptor_get_private_data()->chown (path, uid, gid);
}

static int
adaptor_truncate (const char *path, off_t len)
{
    return _adaptor_get_private_data()->truncate (path, len);
}

static int
adaptor_utime (const char *path, struct utimbuf *buf)
{
    return _adaptor_get_private_data()->utime (path, buf);
}

int
adaptor_ptrToPathInfo (void *_from, struct NetFuse_OpenedPathInfo *to)
{
    struct fuse_file_info *from = _from;
    memset (to, 0, sizeof (*to));
    
    to->flags        = from->flags;
    to->write_page   = from->writepage;
    to->direct_io    = from->direct_io;
    to->keep_cache   = from->keep_cache;
    to->file_handle  = from->fh;
    
    return 0;
}

int
adaptor_pathInfoToPtr (struct NetFuse_OpenedPathInfo *from, void *_to)
{
    struct fuse_file_info *to = _to;
    memset (to, 0, sizeof (*to));
    
    to->flags       = from->flags;
    to->writepage   = from->write_page;
    to->direct_io   = from->direct_io ? 1 : 0;
    to->keep_cache  = from->keep_cache ? 1 : 0;
    to->fh          = from->file_handle;
    
    return 0;
}

static int
adaptor_open (const char *path, struct fuse_file_info *info)
{
    int r;
    
    r = _adaptor_get_private_data()->open (path, info);
    
    return r;
}

static int
adaptor_read (const char *path, char *buf, size_t size, off_t offset,
          struct fuse_file_info *info)
{
    int r, bytesRead = 0;
    
    r = _adaptor_get_private_data()->read (path, (unsigned char*) buf, size, offset,
                                        info, &bytesRead);
    
    if (r == 0 && bytesRead >= 0)
        return bytesRead;
    return r;
}

static int
adaptor_write (const char *path, const char *buf, size_t size, off_t offset,
           struct fuse_file_info *info)
{
    int r = 0;
    int bytesWritten = 0;
    
    WriteHandleCb whcb = _adaptor_get_private_data()->write;

    r = whcb(path, (unsigned char*) buf, size, offset, info, &bytesWritten);
    
    if (r == 0 && bytesWritten >= 0)
        return bytesWritten;
    return r;
}

static int
adaptor_statfs (const char *path, struct statvfs *buf)
{
    return _adaptor_get_private_data()->statfs (path, buf);
}

static int
adaptor_flush (const char *path, struct fuse_file_info *info)
{
    int r;
    
    r = _adaptor_get_private_data()->flush (path, info);
    
    return r;
}

static int
adaptor_release (const char *path, struct fuse_file_info *info)
{
    int r;
    
    r = _adaptor_get_private_data()->release (path, info);
    
    return r;
}

static int
adaptor_fsync (const char *path, int onlyUserData, struct fuse_file_info *info)
{
    int r;
    
    r = _adaptor_get_private_data()->fsync (path, onlyUserData, info);
    
    return r;
}

static int
adaptor_setxattr (const char *path, const char *name, const char *value, size_t size, int flags)
{
    return _adaptor_get_private_data()->setxattr (path, name, (unsigned char*) value, size, flags);
}

static int
adaptor_getxattr (const char *path, const char *name, char *buf, size_t size)
{
    int r, bytesWritten = 0;
    r = _adaptor_get_private_data()->getxattr (path, name, (unsigned char *) buf,
                                            size, &bytesWritten);
    if (r == 0 && bytesWritten >= 0)
        return bytesWritten;
    return r;
}

static int
adaptor_listxattr (const char *path, char *buf, size_t size)
{
    int r, bytesWritten = 0;
    r = _adaptor_get_private_data()->listxattr (path, (unsigned char *) buf, size,
                                             &bytesWritten);
    if (r == 0 && bytesWritten >= 0)
        return bytesWritten;
    return r;
}

static int
adaptor_removexattr (const char *path, const char *name)
{
    return _adaptor_get_private_data()->removexattr (path, name);
}

static int
adaptor_opendir (const char *path, struct fuse_file_info *info)
{
    int r;
    
    r = _adaptor_get_private_data()->opendir (path, info);
    
    return r;
}

static int
adaptor_readdir (const char *path, void* buf, fuse_fill_dir_t filler,
             off_t offset, struct fuse_file_info *info)
{
    int r;
    struct stat stbuf;
    
    r = _adaptor_get_private_data()->readdir (path, buf, filler, offset, info, &stbuf);
    
    return r;
}

int adaptor_invoke_filler (void *_filler, void *buf, const char *path, void *_stbuf, int offset)
{
    struct stat     *stbuf = _stbuf;
    fuse_fill_dir_t filler = _filler;
    
    return filler (buf, path, stbuf, (off_t) offset);
}

static int
adaptor_releasedir (const char *path, struct fuse_file_info *info)
{
    int r;
    
    r = _adaptor_get_private_data()->releasedir (path, info);
    
    return r;
}

static int
adaptor_fsyncdir (const char *path, int onlyUserData, struct fuse_file_info *info)
{
    int r;
    
    r = _adaptor_get_private_data()->fsyncdir (path, onlyUserData, info);
    
    return r;
}

void
adaptor_destroy (void* user_data)
{
    _adaptor_get_private_data()->destroy (user_data);
}

static int
adaptor_access (const char *path, int flags)
{
    return _adaptor_get_private_data()->access (path, flags);
}

static int
adaptor_create (const char *path, mode_t mode, struct fuse_file_info *info)
{
    int r;
    
    r = _adaptor_get_private_data()->create (path, mode, info);
    
    return r;
}

static int
adaptor_ftruncate (const char *path, off_t len, struct fuse_file_info *info)
{
    int r;
    
    r = _adaptor_get_private_data()->ftruncate (path, len, info);
    
    return r;
}

static int
adaptor_fgetattr (const char *path, struct stat *stat, struct fuse_file_info *info)
{
    int r;
    
    r = _adaptor_get_private_data()->fgetattr (path, stat, info);
    
    return r;
}

static int
adaptor_lock (const char *path, struct fuse_file_info *info, int cmd, struct flock *lock)
{
    int r;
    
    r = _adaptor_get_private_data()->lock (path, info, cmd, lock);
    
    return r;
}

static int
adaptor_bmap (const char *path, size_t blocksize, uint64_t *idx)
{
    int r;
    
    r = _adaptor_get_private_data()->bmap (path, blocksize, idx);
    
    return r;
}

static void initalise_fuse_operations_struct (struct NetFuse_Operations *from, struct fuse_operations *to)
{
    memset (to, 0, sizeof(*to));
    
    if (from->getattr)      to->getattr     = adaptor_getattr;
    if (from->readlink)     to->readlink    = adaptor_readlink;
    if (from->mknod)        to->mknod       = adaptor_mknod;
    if (from->mkdir)        to->mkdir       = adaptor_mkdir;
    if (from->unlink)       to->unlink      = adaptor_unlink;
    if (from->rmdir)        to->rmdir       = adaptor_rmdir;
    if (from->symlink)      to->symlink     = adaptor_symlink;
    if (from->rename)       to->rename      = adaptor_rename;
    if (from->renamex)      to->renamex     = adaptor_renamex;
    if (from->link)         to->link        = adaptor_link;
    if (from->chmod)        to->chmod       = adaptor_chmod;
    if (from->chown)        to->chown       = adaptor_chown;
    if (from->truncate)     to->truncate    = adaptor_truncate;
    if (from->utime)        to->utime       = adaptor_utime;
    if (from->open)         to->open        = adaptor_open;
    if (from->read)         to->read        = adaptor_read;
    if (from->write)        to->write       = adaptor_write;
    if (from->statfs)       to->statfs      = adaptor_statfs;
    if (from->flush)        to->flush       = adaptor_flush;
    if (from->release)      to->release     = adaptor_release;
    if (from->fsync)        to->fsync       = adaptor_fsync;
    if (from->setxattr)     to->setxattr    = adaptor_setxattr;
    if (from->getxattr)     to->getxattr    = adaptor_getxattr;
    if (from->listxattr)    to->listxattr   = adaptor_listxattr;
    if (from->removexattr)  to->removexattr = adaptor_removexattr;
    if (from->opendir)      to->opendir     = adaptor_opendir;
    if (from->readdir)      to->readdir     = adaptor_readdir;
    if (from->releasedir)   to->releasedir  = adaptor_releasedir;
    if (from->fsyncdir)     to->fsyncdir    = adaptor_fsyncdir;
    if (from->init)         to->init        = (void* (*)(struct fuse_conn_info*)) from->init;
    if (from->destroy)      to->destroy     = adaptor_destroy;
    if (from->access)       to->access      = adaptor_access;
    if (from->create)       to->create      = adaptor_create;
    if (from->ftruncate)    to->ftruncate   = adaptor_ftruncate;
    if (from->fgetattr)     to->fgetattr    = adaptor_fgetattr;
    if (from->lock)         to->lock        = adaptor_lock;
    if (from->bmap)         to->bmap        = adaptor_bmap;
}

void adaptor_show_fuse_help (const char *appname)
{
    char *help[3];
    char *mountpoint;
    int mt, foreground;
    struct fuse_args args;
    struct fuse_operations ops = {};
    
    help [0] = (char*) appname;
    help [1] = "-ho";
    help [2] = NULL;
    
    memset (&args, 0, sizeof(args));
    
    args.argc = 2;
    args.argv = help;
    args.allocated = 0;
    
    fuse_parse_cmdline (&args, &mountpoint, &mt, &foreground);
    fuse_mount ("mountpoint", &args);
    fuse_new (NULL, &args, &ops, sizeof(ops), NULL);
    
    fuse_opt_free_args (&args);
}

int adaptor_fuse_main(int argc, void *argv, void* ops)
{
    setbuf(stderr, NULL);

    struct fuse_operations fops;
    initalise_fuse_operations_struct((struct NetFuse_Operations*)ops, &fops);
    
    return fuse_main(argc, argv, &fops, NULL);
}


