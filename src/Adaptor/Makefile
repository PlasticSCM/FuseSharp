# FuseSharp
# https://github.com/PlasticSCM/FuseSharp
# 
# make 
# make install
# make clean

CC = gcc
OUT_FILE_NAME = libAdaptor.dylib

CFLAGS = -x c -c -fPIC -O0 -g -Wall -fpermissive -D_FILE_OFFSET_BITS=64
LIBFLAGS = -shared -fPIC 

INC = -I /usr/local/include/osxfuse/  -I /usr/local/Cellar/glib/2.78.3/include/glib-2.0/ -I /usr/local/Cellar/glib/2.78.3/lib/glib-2.0/include/
LIB = -L /usr/local/lib/

OBJ_DIR=./obj
OUT_DIR=./lib
INSTALL_DIR=/usr/local/lib/


$(OUT_FILE_NAME): $(patsubst %.c,$(OBJ_DIR)/%.o,$(wildcard *.c))
	$(CC) $(LIBFLAGS) -o $(OUT_DIR)/$(OUT_FILE_NAME) $(LIB) -l fuse $(OBJ_DIR)/*.o -lc


$(OBJ_DIR)/%.o: %.c dirmake
	$(CC) -c $(INC) $(CFLAGS) -o $@  $<


dirmake:
	@mkdir -p $(OUT_DIR)
	@mkdir -p $(OBJ_DIR)
	
clean:
	rm -rf $(OBJ_DIR) $(OUT_DIR) Makefile.bak
	
install:
	cp $(OUT_DIR)/$(OUT_FILE_NAME) $(INSTALL_DIR)
	
deinstall:
	rm -f $(INSTALL_DIR)/$(OUT_FILE_NAME)

rebuild: clean build


