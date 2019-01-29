//
//  adaptor.h
//  Adaptor
//
//  Created by groberts on 15/08/2018.
//  Copyright © 2018 groberts. All rights reserved.
//

#ifndef adaptor_h
#define adaptor_h

#define FUSE_USE_VERSION 26

#include "definitions.h"

int __cdecl adaptor_fuse_main(int argc, void *argv, void* ops);
int __cdecl adaptor_invoke_filler(void *_filler, void *buf, const char *path, void *_stbuf, int offset);
int __cdecl adaptor_ptrToPathInfo(void *_from, struct NetFuse_OpenedPathInfo *to);
int __cdecl adaptor_pathInfoToPtr(struct NetFuse_OpenedPathInfo *from, void *_to);

#endif /* adaptor_h */
