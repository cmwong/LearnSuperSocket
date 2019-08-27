extern crate byteorder;

use std::io::prelude::*;
use std::net::TcpStream;
use std::{thread, time};

use byteorder::{BigEndian, LittleEndian, ByteOrder, WriteBytesExt};

static NTHREADS: i32 = 2;

fn main() {
  for i in 0..NTHREADS {
    let _ = thread::spawn(move || {
      let mut stream = TcpStream::connect("127.0.0.1:8800").unwrap();

      loop {
        let msg = format!("the answer is {}", i);

        println!("thread {}: Sending over message length of {}", i, msg.len());
        // let mut buf = [0u8; 8];
        // BigEndian::write_u64(&mut buf, msg.len() as u64);
        // BigEndian::write_u16(&mut buf[2..3], msg.len() as u16);

        // stream.write_all(buf.as_ref()).unwrap();
        // stream.write_all(msg.as_ref()).unwrap();

        let mut buf = vec![];
        buf.write_u16::<LittleEndian>(0).unwrap();
        buf.write_u16::<LittleEndian>((msg.len() + 8) as u16).unwrap();
        buf.write_u16::<LittleEndian>(0).unwrap();
        buf.write_u16::<LittleEndian>(0).unwrap();

        stream.write_all(buf.as_ref()).unwrap();
        stream.write_all(msg.as_ref()).unwrap();


        let mut buf = [0u8; 8];
        let _ = stream.read(&mut buf).unwrap();

        // let msg_len = BigEndian::read_u64(&buf);
        let  msg_len = LittleEndian::read_u16(&buf[2..4]) - 8;
        println!("thread {}: Reading message length of {}", i, msg_len);

        let mut r = [0u8; 256];
        let s_ref = <TcpStream as Read>::by_ref(&mut stream);

        match s_ref.take(msg_len as u64).read(&mut r) {
          Ok(0) => {
            println!("thread {}: 0 bytes read", i);
          }
          Ok(n) => {
            println!("thread {}: {} bytes read", i, n);

            let s = std::str::from_utf8(&r[..]).unwrap();
            println!("thread {} read = {}", i, s);
          }
          Err(e) => {
            panic!("thread {}: {}", i, e);
          }
        }
      }
    });
  }

  loop {
    thread::sleep(time::Duration::from_millis(3000));
  }
}
