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

INC = -I /usr/local/include/osxfuse/ $(shell pkg-config --cflags glib-2.0)
LIBS = -L /usr/local/lib/ $(shell pkg-config --libs glib-2.0)

OBJ_DIR=./obj
OUT_DIR=./lib
INSTALL_DIR=/usr/local/lib/


$(OUT_FILE_NAME): $(patsubst %.c,$(OBJ_DIR)/%.o,$(wildcard *.c))
	$(CC) $(LIBFLAGS) -o $(OUT_DIR)/$(OUT_FILE_NAME) $(LIBS) -l fuse $(OBJ_DIR)/*.o -lc


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


