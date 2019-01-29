
#ifdef HAVE_CONFIG_H
#include <config.h>
#endif /* ndef HAVE_CONFIG_H */

#include <stdlib.h>
#include <string.h>

#ifndef FUSE_USE_VERSION
#define FUSE_USE_VERSION 26
#endif

/*
 * Implementation Includes
 */
#include <fuse.h>

#include "definitions.h"

#include <errno.h>    /* errno, EOVERFLOW */
#include <glib.h>     /* g* types, g_assert_not_reached() */

#if defined (G_MININT8)
#define CNM_MININT8 G_MININT8
#else
#define CNM_MININT8 (-128)
#endif

#if defined (G_MAXINT8)
#define CNM_MAXINT8 G_MAXINT8
#else
#define CNM_MAXINT8 (127)
#endif

#if defined (G_MAXUINT8)
#define CNM_MAXUINT8 G_MAXUINT8
#else
#define CNM_MAXUINT8 (255)
#endif

#if defined (G_MININT16)
#define CNM_MININT16 G_MININT16
#else
#define CNM_MININT16 (-32768)
#endif

#if defined (G_MAXINT16)
#define CNM_MAXINT16 G_MAXINT16
#else
#define CNM_MAXINT16 (32767)
#endif

#if defined (G_MAXUINT16)
#define CNM_MAXUINT16 G_MAXUINT16
#else
#define CNM_MAXUINT16 (65535)
#endif

#if defined (G_MININT32)
#define CNM_MININT32 G_MININT32
#else
#define CNM_MININT32 (-2147483648)
#endif

#if defined (G_MAXINT32)
#define CNM_MAXINT32 G_MAXINT32
#else
#define CNM_MAXINT32 (2147483647)
#endif

#if defined (G_MAXUINT32)
#define CNM_MAXUINT32 G_MAXUINT32
#else
#define CNM_MAXUINT32 (4294967295U)
#endif

#if defined (G_MININT64)
#define CNM_MININT64 G_MININT64
#else
#define CNM_MININT64 (-9223372036854775808LL)
#endif

#if defined (G_MAXINT64)
#define CNM_MAXINT64 G_MAXINT64
#else
#define CNM_MAXINT64 (9223372036854775807LL)
#endif

#if defined (G_MAXUINT64)
#define CNM_MAXUINT64 G_MAXUINT64
#else
#define CNM_MAXUINT64 (18446744073709551615ULL)
#endif


/* returns TRUE if @type is an unsigned type */
#define _cnm_integral_type_is_unsigned(type) \
    (sizeof(type) == sizeof(gint8)           \
      ? (((type)-1) > CNM_MAXINT8)             \
      : sizeof(type) == sizeof(gint16)       \
        ? (((type)-1) > CNM_MAXINT16)          \
        : sizeof(type) == sizeof(gint32)     \
          ? (((type)-1) > CNM_MAXINT32)        \
          : sizeof(type) == sizeof(gint64)   \
            ? (((type)-1) > CNM_MAXINT64)      \
            : (g_assert_not_reached (), 0))

/* returns the minimum value of @type as a gint64 */
#define _cnm_integral_type_min(type)          \
    (_cnm_integral_type_is_unsigned (type)    \
      ? 0                                     \
      : sizeof(type) == sizeof(gint8)         \
        ? CNM_MININT8                           \
        : sizeof(type) == sizeof(gint16)      \
          ? CNM_MININT16                        \
          : sizeof(type) == sizeof(gint32)    \
            ? CNM_MININT32                      \
            : sizeof(type) == sizeof(gint64)  \
              ? CNM_MININT64                    \
              : (g_assert_not_reached (), 0))

/* returns the maximum value of @type as a guint64 */
#define _cnm_integral_type_max(type)            \
    (_cnm_integral_type_is_unsigned (type)      \
      ? sizeof(type) == sizeof(gint8)           \
        ? CNM_MAXUINT8                            \
        : sizeof(type) == sizeof(gint16)        \
          ? CNM_MAXUINT16                         \
          : sizeof(type) == sizeof(gint32)      \
            ? CNM_MAXUINT32                       \
            : sizeof(type) == sizeof(gint64)    \
              ? CNM_MAXUINT64                     \
              : (g_assert_not_reached (), 0)    \
      : sizeof(type) == sizeof(gint8)           \
          ? CNM_MAXINT8                           \
          : sizeof(type) == sizeof(gint16)      \
            ? CNM_MAXINT16                        \
            : sizeof(type) == sizeof(gint32)    \
              ? CNM_MAXINT32                      \
              : sizeof(type) == sizeof(gint64)  \
                ? CNM_MAXINT64                    \
                : (g_assert_not_reached (), 0))

#ifdef _CNM_DUMP
#define _cnm_dump(to_t,from)                                             \
  printf ("# %s -> %s: uns=%i; min=%llx; max=%llx; value=%llx; lt=%i; l0=%i; gt=%i; e=%i\n", \
    #from, #to_t,                                                        \
    (int) _cnm_integral_type_is_unsigned (to_t),                         \
    (gint64) (_cnm_integral_type_min (to_t)),                            \
    (gint64) (_cnm_integral_type_max (to_t)),                            \
    (gint64) (from),                                                     \
    (((gint64) _cnm_integral_type_min (to_t)) <= (gint64) from),         \
    (from < 0),                                                          \
    (((guint64) from) <= (guint64) _cnm_integral_type_max (to_t)),       \
    !((int) _cnm_integral_type_is_unsigned (to_t)                        \
      ? ((0 <= from) &&                                                  \
         ((guint64) from <= (guint64) _cnm_integral_type_max (to_t)))    \
      : ((gint64) _cnm_integral_type_min(to_t) <= (gint64) from &&       \
         (guint64) from <= (guint64) _cnm_integral_type_max (to_t)))     \
  )
#else /* ndef _CNM_DUMP */
#define _cnm_dump(to_t, from) do {} while (0)
#endif /* def _CNM_DUMP */

#ifdef DEBUG
#define _cnm_return_val_if_overflow(to_t,from,val)  G_STMT_START {   \
    int     uns = _cnm_integral_type_is_unsigned (to_t);             \
    gint64  min = (gint64)  _cnm_integral_type_min (to_t);           \
    guint64 max = (guint64) _cnm_integral_type_max (to_t);           \
    gint64  sf  = (gint64)  from;                                    \
    guint64 uf  = (guint64) from;                                    \
    if (!(uns ? ((0 <= from) && (uf <= max))                         \
              : (min <= sf && (from < 0 || uf <= max)))) {           \
      _cnm_dump(to_t, from);                                         \
      errno = EOVERFLOW;                                             \
      return (val);                                                  \
    }                                                                \
  } G_STMT_END
#else /* !def DEBUG */
/* don't do any overflow checking */
#define _cnm_return_val_if_overflow(to_t,from,val)  G_STMT_START {   \
  } G_STMT_END
#endif /* def DEBUG */

#ifdef HAVE_STRUCT_FUSE_ARGS
int
NetFuseArgsToArgs (struct NetFuse_Args *from, struct fuse_args *to)
{
	_cnm_return_val_if_overflow (int, from->argc, -1);
	_cnm_return_val_if_overflow (int, from->allocated, -1);

	memset (to, 0, sizeof(*to));

	to->argc      = from->argc;
	to->argv      = from->argv;
	to->allocated = from->allocated;

	return 0;
}
#endif /* ndef HAVE_STRUCT_FUSE_ARGS */


#ifdef HAVE_STRUCT_FUSE_ARGS
int
ArgsToNetFuseArgs (struct fuse_args *from, struct NetFuse_Args *to)
{
	_cnm_return_val_if_overflow (int, from->argc, -1);
	_cnm_return_val_if_overflow (int, from->allocated, -1);

	memset (to, 0, sizeof(*to));

	to->argc      = from->argc;
	to->argv      = from->argv;
	to->allocated = from->allocated;

	return 0;
}
#endif /* ndef HAVE_STRUCT_FUSE_ARGS */


